namespace Amazon.Kinesis.ClientLibrary
{
    internal class DefaultShutdownInput : ShutdownInput
    {
        private readonly ShutdownReason _reason;
        private readonly Checkpointer _checkpointer;

        public ShutdownReason Reason { get { return _reason; } }
        public Checkpointer Checkpointer { get { return _checkpointer; } }

        public DefaultShutdownInput(ShutdownReason reason, Checkpointer checkpointer)
        {
            _reason = reason;
            _checkpointer = checkpointer;
        }
    }
}