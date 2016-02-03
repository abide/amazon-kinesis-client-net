using System.Runtime.Serialization;

namespace Amazon.Kinesis.ClientLibrary
{
    [DataContract]
    internal class CheckpointAction : Action
    {
        [DataMember(Name = "checkpoint")]
        public string SequenceNumber { get; set; }

        [DataMember(Name = "error", IsRequired = false)]
        public string Error { get; set; }

        public CheckpointAction(string sequenceNumber)
        {
            Type = "checkpoint";
            SequenceNumber = sequenceNumber;
        }
    }
}