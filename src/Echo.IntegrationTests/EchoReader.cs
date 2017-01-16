using System.Collections.Generic;
using System.IO;

namespace Echo.IntegrationTests
{
    internal class EchoReader : IEchoReader
    {
        private readonly StreamReader _streamReader;

        internal EchoReader(StreamReader streamReader)
        {
            _streamReader = streamReader;
        }

        public string ReadLine()
        {
            return _streamReader.ReadLine();
        }
    }
}
