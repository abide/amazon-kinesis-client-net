using System;
using System.Linq;
using System.Runtime.Serialization;

namespace Amazon.Kinesis.ClientLibrary
{
    [DataContract]
    internal class StatusAction : Action
    {
        [DataMember(Name = "responseFor")]
        public string ResponseFor { get; set; }

        public StatusAction(Type t)
            : this((string) Types.Where(x => x.Value == t).Select(x => x.Key).First())
        {
        }

        public StatusAction(string type)
        {
            Type = "status";
            ResponseFor = type;
        }
    }
}