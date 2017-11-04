using System.IO;

namespace Echo.IntegrationTests
{
    public class EchoReader : IEchoReader
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
