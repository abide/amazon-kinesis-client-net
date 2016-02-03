namespace Amazon.Kinesis.ClientLibrary
{
    internal class DefaultInitializationInput : InitializationInput
    {
        private readonly string _shardId;

        public string ShardId { get { return _shardId; } }

        public DefaultInitializationInput(string shardId)
        {
            _shardId = shardId;
        }
    }
}