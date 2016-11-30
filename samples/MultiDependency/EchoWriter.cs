using Echo;
using System.IO;

namespace Samples.MultiDependency
{
    internal class EchoWriter : IEchoWriter
    {
        private readonly StreamWriter _streamWriter;

        public EchoWriter(StreamWriter streamWriter)
        {
            _streamWriter = streamWriter;
        }

        public void WriteSerializedInvocation(string serializedInvocation)
        {
            _streamWriter.WriteLine(serializedInvocation);
        }
    }
}
