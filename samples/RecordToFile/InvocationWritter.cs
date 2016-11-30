using Echo;
using System.IO;

namespace Samples.RecordToFile
{
    internal class InvocationWritter : IInvocationEntryWritter
    {
        private readonly StreamWriter _streamWriter;

        public InvocationWritter(StreamWriter streamWriter)
        {
            _streamWriter = streamWriter;
        }

        public void WriteSerializedInvocation(string serializedInvocation)
        {
            _streamWriter.WriteLine(serializedInvocation);
        }
    }
}
