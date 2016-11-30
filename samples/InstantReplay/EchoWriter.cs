using Echo;
using System.IO;

namespace Samples.InstantReplay
{
    internal class EchoWriter : IEchoWriter
    {
        private readonly StreamWriter _streamWriter;

        internal EchoWriter(StreamWriter streamWriter)
        {
            _streamWriter = streamWriter;
        }

        public void WriteSerializedInvocation(string serializedInvocation)
        {
            _streamWriter.WriteLine(serializedInvocation);
        }
    }
}
