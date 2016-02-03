using System;
using System.Collections.Generic;
using Stateless;

namespace Amazon.Kinesis.ClientLibrary
{
    internal class DefaultKclProcess : KclProcess
    {
        private enum State
        {
            Start,
            Ready,
            Checkpointing,
            FinalCheckpointing,
            ShuttingDown,
            End,
            Processing,
            Initializing
        }

        private enum Trigger
        {
            BeginCheckpoint,
            BeginInitialize,
            BeginProcessRecords,
            BeginShutdown,
            FinishCheckpoint,
            FinishInitialize,
            FinishProcessRecords,
            FinishShutdown
        }

        private class InternalCheckpointer : Checkpointer
        {
            private readonly DefaultKclProcess _p;

            public InternalCheckpointer(DefaultKclProcess p)
            {
                _p = p;
            }

            internal override void Checkpoint(string sequenceNumber, CheckpointErrorHandler errorHandler = null)
            {
                _p.CheckpointSeqNo = sequenceNumber;
                _p._stateMachine.Fire(Trigger.BeginCheckpoint);
                if (_p.CheckpointError != null && errorHandler != null)
                {
                    errorHandler.Invoke(sequenceNumber, _p.CheckpointError, this);
                }
            }
        }

        private readonly StateMachine<State, Trigger> _stateMachine = new StateMachine<State, Trigger>(State.Start);
       
        private readonly IRecordProcessor _processor;
        private readonly IoHandler _iohandler;
        private readonly Checkpointer _checkpointer;

        private string CheckpointSeqNo { get; set; }

        private string CheckpointError { get; set; }

        private string ShardId { get; set; }

        private ShutdownReason ShutdownReason { get; set; }

        private List<Record> Records { get; set; }

        internal DefaultKclProcess(IRecordProcessor processor, IoHandler iohandler)
        {
            _processor = processor;
            _iohandler = iohandler;
            _checkpointer = new InternalCheckpointer(this);
            ConfigureStateMachine();
        }

        public override void Run()
        {
            while (ProcessNextLine())
            {
            }
        }

        private bool ProcessNextLine()
        {
            Action a = _iohandler.ReadAction();
            if (a != null)
            {
                DispatchAction(a);
                return true;
            }
            else
            {
                return false;
            }
        }

        private void DispatchAction(Action a)
        {
            var initAction = a as InitializeAction;
            if (initAction != null)
            {
                ShardId = initAction.ShardId;
                _stateMachine.Fire(Trigger.BeginInitialize);
                return;
            }

            var processRecordAction = a as ProcessRecordsAction;
            if (processRecordAction != null)
            {
                Records = processRecordAction.Records;
                _stateMachine.Fire(Trigger.BeginProcessRecords);
                return;
            }

            var shutdownAction = a as ShutdownAction;
            if (shutdownAction != null)
            {
                ShutdownReason = (ShutdownReason) Enum.Parse(typeof(ShutdownReason), shutdownAction.Reason);
                _stateMachine.Fire(Trigger.BeginShutdown);
                return;
            }

            var checkpointAction = a as CheckpointAction;
            if (checkpointAction != null)
            {
                CheckpointError = checkpointAction.Error;
                CheckpointSeqNo = checkpointAction.SequenceNumber;
                _stateMachine.Fire(Trigger.FinishCheckpoint);
                return;
            }

            throw new MalformedActionException("Received an action which couldn't be understood: " + a.Type);       
        }

        private void ConfigureStateMachine()
        {
            _stateMachine.OnUnhandledTrigger((state, trigger) =>
            {
                throw new MalformedActionException("trigger " + trigger + " is invalid for state " + state);
            });

            // Uncomment to help debugging
            // _stateMachine.OnTransitioned(t => Console.Error.WriteLine(t.Source + " --> " + t.Destination));

            _stateMachine.Configure(State.Start)
                .Permit(Trigger.BeginInitialize, State.Initializing);

            _stateMachine.Configure(State.Ready)
                .OnEntryFrom(Trigger.FinishProcessRecords, FinishProcessRecords)
                .OnEntryFrom(Trigger.FinishInitialize, FinishInitialize)
                .Permit(Trigger.BeginProcessRecords, State.Processing)
                .Permit(Trigger.BeginShutdown, State.ShuttingDown);

            _stateMachine.Configure(State.Checkpointing)
                .OnEntryFrom(Trigger.BeginCheckpoint, BeginCheckpoint)
                .Permit(Trigger.FinishCheckpoint, State.Processing);

            _stateMachine.Configure(State.FinalCheckpointing)
                .OnEntryFrom(Trigger.BeginCheckpoint, BeginCheckpoint)
                .Permit(Trigger.FinishCheckpoint, State.ShuttingDown);

            _stateMachine.Configure(State.ShuttingDown)
                .OnEntryFrom(Trigger.BeginShutdown, BeginShutdown)
                .OnEntryFrom(Trigger.FinishCheckpoint, FinishCheckpoint)
                .Permit(Trigger.FinishShutdown, State.End)
                .Permit(Trigger.BeginCheckpoint, State.FinalCheckpointing);

            _stateMachine.Configure(State.End)
                .OnEntryFrom(Trigger.FinishShutdown, FinishShutdown);

            _stateMachine.Configure(State.Processing)
                .OnEntryFrom(Trigger.BeginProcessRecords, BeginProcessRecords)
                .OnEntryFrom(Trigger.FinishCheckpoint, FinishCheckpoint)
                .Permit(Trigger.BeginCheckpoint, State.Checkpointing)
                .Permit(Trigger.FinishProcessRecords, State.Ready);

            _stateMachine.Configure(State.Initializing)
                .OnEntryFrom(Trigger.BeginInitialize, BeginInitialize)
                .Permit(Trigger.FinishInitialize, State.Ready);
        }

        private void BeginInitialize()
        {
            _processor.Initialize(new DefaultInitializationInput(ShardId));
            _stateMachine.Fire(Trigger.FinishInitialize);
        }

        private void FinishInitialize()
        {
            _iohandler.WriteAction(new StatusAction(typeof(InitializeAction)));
        }

        private void BeginShutdown()
        {
            _processor.Shutdown(new DefaultShutdownInput(ShutdownReason, _checkpointer));
            _stateMachine.Fire(Trigger.FinishShutdown);
        }

        private void FinishShutdown()
        {
            _iohandler.WriteAction(new StatusAction(typeof(ShutdownAction)));
        }

        private void BeginProcessRecords()
        {
            _processor.ProcessRecords(new DefaultProcessRecordsInput(Records, _checkpointer));
            _stateMachine.Fire(Trigger.FinishProcessRecords);
        }

        private void FinishProcessRecords()
        {
            _iohandler.WriteAction(new StatusAction(typeof(ProcessRecordsAction)));
        }

        private void BeginCheckpoint()
        {
            _iohandler.WriteAction(new CheckpointAction(CheckpointSeqNo));
            ProcessNextLine();
        }

        private void FinishCheckpoint()
        {
            // nothing to do here
        }
    }
}