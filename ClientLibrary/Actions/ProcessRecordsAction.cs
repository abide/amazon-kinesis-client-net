using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Amazon.Kinesis.ClientLibrary
{
    [DataContract]
    internal class ProcessRecordsAction : Action
    {
        [DataMember(Name = "records")]
        private List<DefaultRecord> _actualRecords;

        public List<Record> Records
        {
            get
            {
                return _actualRecords.Select(x => x as Record).ToList();
            }
        }

        public ProcessRecordsAction(params DefaultRecord[] records)
        {
            Type = "processRecords";
            _actualRecords = records.ToArray().ToList();
        }
    }
}