using System.Runtime.Serialization;

namespace Amazon.Kinesis.ClientLibrary
{
    [DataContract]
    internal class ShutdownAction : Action
    {
        [DataMember(Name = "reason")]
        public string Reason { get; set; }

        public ShutdownAction(string reason)
        {
            Type = "shutdown";
            Reason = reason;
        }
    }
}