using Echo;
using System.Reflection;

namespace Samples.InstantReplay.Storage
{
    internal class Record
    {
        public MethodInfo Method { get; }

        public object[] Arguments { get; }

        public InvocationResult ReturnValue { get; }

        public Record(MethodInfo methodInfo, InvocationResult returnValue, object[] arguments)
        {
            Method = methodInfo;
            Arguments = arguments;
            ReturnValue = returnValue;
        }
    }
}
