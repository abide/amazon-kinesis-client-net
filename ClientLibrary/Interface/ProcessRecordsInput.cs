using System.Collections.Generic;

namespace Amazon.Kinesis.ClientLibrary
{
    /// <summary>
    /// Contains a batch of records to be processed, along with contextual information.
    /// </summary>
    public interface ProcessRecordsInput
    {
        /// <summary>
        /// Get the records to be processed.
        /// </summary>
        /// <value>The records.</value>
        List<Record> Records { get; }

        /// <summary>
        /// Gets the checkpointer.
        /// </summary>
        /// <value>The checkpointer.</value>
        Checkpointer Checkpointer { get; }
    }
}