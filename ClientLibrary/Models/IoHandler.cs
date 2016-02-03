using System;
using System.IO;

namespace Amazon.Kinesis.ClientLibrary
{
    internal class IoHandler : IDisposable
    {
        private readonly StreamReader _reader;
        private readonly StreamWriter _outWriter;
        private readonly StreamWriter _errorWriter;

        public IoHandler()
            : this(Console.OpenStandardInput(), Console.OpenStandardOutput(), Console.OpenStandardError())
        {
        }

        public IoHandler(Stream inStream, Stream outStream, Stream errStream)
        {
            _reader = new StreamReader(inStream);
            _outWriter = new StreamWriter(outStream);
            _errorWriter = new StreamWriter(errStream);
        }

        public void WriteAction(Action a)
        {
            _outWriter.WriteLine(a.ToString());
            _outWriter.Flush();
        }

        public Action ReadAction()
        {
            string s = _reader.ReadLine();
            if (s == null)
            {
                return null;
            }
            return Action.Parse(s);
        }

        public void WriteError(string message, Exception e)
        {
            _errorWriter.WriteLine(message);
            _errorWriter.WriteLine(e.StackTrace);
            _errorWriter.Flush();
        }

        public void Dispose()
        {
            _reader.Dispose();
            _outWriter.Dispose();
            _errorWriter.Dispose();
        }
    }
}