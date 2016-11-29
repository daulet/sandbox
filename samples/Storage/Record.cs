using System.Reflection;

namespace Samples.Storage
{
    internal class Record
    {
        public MethodInfo Method { get; }

        public object[] Arguments { get; }

        public object ReturnValue { get; }

        public Record(MethodInfo methodInfo, object returnValue, object[] arguments)
        {
            Method = methodInfo;
            Arguments = arguments;
            ReturnValue = returnValue;
        }
    }
}
