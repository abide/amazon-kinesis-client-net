using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Amazon.Kinesis.ClientLibrary
{
    [DataContract]
    internal class Action
    {
        protected static readonly Dictionary<string, Type> Types = new Dictionary<String, Type>()
        {
            { "initialize", typeof(InitializeAction) },
            { "processRecords", typeof(ProcessRecordsAction) },
            { "shutdown", typeof(ShutdownAction) },
            { "checkpoint", typeof(CheckpointAction) },
            { "status", typeof(StatusAction) }
        };

        public static Action Parse(string json)
        {
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                try
                {
                    // Deserialize just the action field to get the type
                    var jsonSerializer = new DataContractJsonSerializer(typeof(Action));
                    var a = jsonSerializer.ReadObject(ms) as Action;
                    // Deserialize again into the appropriate type
                    ms.Position = 0;
                    jsonSerializer = new DataContractJsonSerializer(Types[a.Type]);
                    a = jsonSerializer.ReadObject(ms) as Action;
                    return a;
                }
                catch (Exception e)
                {
                    ms.Position = 0;
                    throw new MalformedActionException("Received an action which couldn't be understood: " + json, e);
                }
            }
        }

        [DataMember(Name = "action")]
        public string Type { get; set; }

        public override string ToString()
        {
            var jsonSerializer = new DataContractJsonSerializer(GetType());
            var ms = new MemoryStream();
            jsonSerializer.WriteObject(ms, this);
            ms.Position = 0;
            using (StreamReader sr = new StreamReader(ms))
            {
                return sr.ReadLine();
            }
        }
    }
}