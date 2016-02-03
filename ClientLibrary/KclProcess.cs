namespace Amazon.Kinesis.ClientLibrary
{
    /// <summary>
    /// Instances of KclProcess communicate with the multi-lang daemon. The Main method of your application must
    /// create an instance of KclProcess and call its Run method.
    /// </summary>
    public abstract class KclProcess
    {
        /// <summary>
        /// Create an instance of KclProcess that uses the given IRecordProcessor to process records.
        /// </summary>
        /// <param name="recordProcessor">IRecordProcessor used to process records.</param>
        public static KclProcess Create(IRecordProcessor recordProcessor)
        {
            return Create(recordProcessor, new IoHandler());
        }

        internal static KclProcess Create(IRecordProcessor recordProcessor, IoHandler ioHandler)
        {
            return new DefaultKclProcess(recordProcessor, ioHandler);
        }

        /// <summary>
        /// Starts the KclProcess. Once this method is called, the KclProcess instance will continuously communicate with
        /// the multi-lang daemon, performing actions as appropriate. This method blocks until it detects that the
        /// multi-lang daemon has terminated the communication.
        /// </summary>
        public abstract void Run();
    }
}