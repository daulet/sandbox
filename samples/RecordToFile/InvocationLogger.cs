using Echo;
using System.IO;

namespace Samples.RecordToFile
{
    internal class InvocationLogger : IInvocationLogger
    {
        private readonly StreamWriter _streamWriter;

        public InvocationLogger(StreamWriter streamWriter)
        {
            _streamWriter = streamWriter;
        }

        public void WriteSerializedInvocation(string serializedInvocation)
        {
            _streamWriter.WriteLine(serializedInvocation);
        }
    }
}
