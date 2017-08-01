using Echo;
using System.Collections.Generic;

namespace Samples.InstantReplay
{
    internal class EchoReader : IEchoReader
    {
        private readonly IList<string> _records;
        private int _lastReadIndex;

        internal EchoReader(IList<string> records)
        {
            _records = records;
            _lastReadIndex = 0;
        }

        public string ReadLine()
        {
            if (_lastReadIndex < _records.Count)
            {
                return _records[_lastReadIndex++];
            }
            return null;
        }
    }
}
