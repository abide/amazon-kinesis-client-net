namespace Amazon.Kinesis.ClientLibrary
{
    /// <summary>
    /// Contextual information that can used to perform specialized initialization for this IRecordProcessor.
    /// </summary>
    public interface InitializationInput
    {
        /// <summary>
        /// Gets the shard identifier.
        /// </summary>
        /// <value>The shard identifier. Each IRecordProcessor processes records from one and only one shard.</value>
        string ShardId { get; }
    }
}