using System;
using System.Runtime.Serialization;
using System.Text;

namespace Amazon.Kinesis.ClientLibrary
{
    [DataContract]
    internal class DefaultRecord : Record
    {
        [DataMember(Name = "sequenceNumber")]
        private string _sequenceNumber;

        [DataMember(Name = "data")]
        private string _base64;
        private byte[] _data;

        [DataMember(Name = "partitionKey")]
        private string _partitionKey;

        public override string PartitionKey { get { return _partitionKey; } }

        public override byte[] Data
        {
            get
            {
                if (_data == null)
                {
                    _data = Convert.FromBase64String(_base64);
                    _base64 = null;
                }
                return _data;
            }
        }

        public override string SequenceNumber { get { return _sequenceNumber; } }

        public DefaultRecord(string sequenceNumber, string partitionKey, string data)
        {
            _data = Encoding.UTF8.GetBytes(data);
            _sequenceNumber = sequenceNumber;
            _partitionKey = partitionKey;
        }
    }
}