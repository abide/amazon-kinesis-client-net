using System;

namespace Amazon.Kinesis.ClientLibrary
{
    internal class MalformedActionException : Exception
    {
        public MalformedActionException(string message)
            : base(message)
        {
        }

        public MalformedActionException(string message, Exception cause)
            : base(message, cause)
        {
        }
    }
}