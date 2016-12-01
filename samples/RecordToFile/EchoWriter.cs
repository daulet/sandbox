using Echo;
using System.IO;

namespace Samples.RecordToFile
{
    internal class EchoWriter : IEchoWriter
    {
        private readonly StreamWriter _streamWriter;

        public EchoWriter(StreamWriter streamWriter)
        {
            _streamWriter = streamWriter;
        }

        public void WriteEcho(string echo)
        {
            _streamWriter.WriteLine(echo);
        }
    }
}
