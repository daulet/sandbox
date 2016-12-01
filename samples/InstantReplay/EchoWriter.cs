using Echo;
using System.Collections.Generic;

namespace Samples.InstantReplay
{
    internal class EchoWriter : IEchoWriter
    {
        private readonly IList<string> _records;

        internal EchoWriter(IList<string> records)
        {
            _records = records;
        }

        public void WriteEcho(string echo)
        {
            _records.Add(echo);
        }
    }
}
