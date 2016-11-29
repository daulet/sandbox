using Echo;
using System.Reflection;

namespace Samples.Storage
{
    class TapeWritter : IInvocationWritter
    {
        private readonly Tape _tape = new Tape();

        public void RecordInvocation(MethodInfo methodInfo, object returnValue, object[] arguments)
        {
            _tape.AddRecord(new Record(methodInfo, returnValue, arguments));
        }
    }
}
