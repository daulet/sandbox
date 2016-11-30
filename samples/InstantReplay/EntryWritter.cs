using Echo;
using System.IO;

namespace Samples.InstantReplay
{
    internal class EntityWritter : IInvocationEntryWritter
    {
        private readonly StreamWriter _streamWriter;

        internal EntityWritter(StreamWriter streamWriter)
        {
            _streamWriter = streamWriter;
        }

        public void WriteSerializedInvocation(string serializedInvocation)
        {
            _streamWriter.WriteLine(serializedInvocation);
        }
    }
}
