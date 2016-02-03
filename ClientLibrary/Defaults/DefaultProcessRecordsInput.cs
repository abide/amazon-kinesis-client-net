using System.Collections.Generic;

namespace Amazon.Kinesis.ClientLibrary
{
    internal class DefaultProcessRecordsInput : ProcessRecordsInput
    {
        private readonly List<Record> _records;
        private readonly Checkpointer _checkpointer;

        public List<Record> Records { get { return _records; } }
        public Checkpointer Checkpointer { get { return _checkpointer; } }

        public DefaultProcessRecordsInput(List<Record> records, Checkpointer checkpointer) {
            _records = records;
            _checkpointer = checkpointer;
        }
    }
}