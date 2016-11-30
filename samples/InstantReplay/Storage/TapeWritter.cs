using Echo;
using System.Reflection;

namespace Samples.InstantReplay.Storage
{
    internal class TapeWritter : IInvocationWritter
    {
        private readonly Tape _tape = new Tape();

        public Tape GeTape()
        {
            return _tape;
        }

        public void WriteInvocation(MethodInfo methodInfo, InvocationResult invocationResult, object[] arguments)
        {
            _tape.AddRecord(new Record(methodInfo, invocationResult, arguments));
        }
    }
}
