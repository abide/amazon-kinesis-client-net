namespace Amazon.Kinesis.ClientLibrary
{
    /// <summary>
    /// Contextual information that can used to perform specialized shutdown procedures for this IRecordProcessor.
    /// </summary>
    public interface ShutdownInput {
        /// <summary>
        /// Gets the shutdown reason.
        /// </summary>
        /// <value>The shutdown reason.</value>
        ShutdownReason Reason { get; }

        /// <summary>
        /// Gets the checkpointer.
        /// </summary>
        /// <value>The checkpointer.</value>
        Checkpointer Checkpointer { get; }
    }
}