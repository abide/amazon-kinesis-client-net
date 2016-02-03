namespace Amazon.Kinesis.ClientLibrary
{
    /// <summary>
    /// Instances of this delegate can be passed to Checkpointer's Checkpoint methods. The delegate will be
    /// invoked when a checkpoint operation fails.
    /// </summary>
    /// <param name="sequenceNumber">The sequence number at which the checkpoint was attempted.</param>
    /// <param name="errorMessage">The error message received from the checkpoint failure.</param>
    /// <param name="checkpointer">The Checkpointer instance that was used to perform the checkpoint operation.</param>
    public delegate void CheckpointErrorHandler(string sequenceNumber, string errorMessage, Checkpointer checkpointer);
}