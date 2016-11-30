using System;
using System.Reflection;

namespace Echo.Serialization
{
    internal class InvocationEntry
    {
        public object[] Arguments { get; }

        public string Method { get; }

        public InvocationResult ReturnValue { get; }

        public DateTimeOffset Timestamp { get; }

        public InvocationEntry(DateTimeOffset timestamp, object[] arguments, MethodInfo methodInfo, InvocationResult returnValue)
        {
            Arguments = arguments;
            Method = methodInfo.Name;
            ReturnValue = returnValue;
            Timestamp = timestamp;
        }
    }
}
