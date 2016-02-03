using System.Runtime.Serialization;

namespace Amazon.Kinesis.ClientLibrary
{
    [DataContract]
    internal class InitializeAction : Action
    {
        [DataMember(Name = "shardId")]
        public string ShardId { get; set; }

        public InitializeAction(string shardId)
        {
            Type = "initialize";
            ShardId = shardId;
        }
    }
}