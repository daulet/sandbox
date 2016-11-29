using Echo;
using System;
using System.Reflection;

namespace Samples.RecordToFile.Storage
{
    internal class InvocationLog
    {
        public object[] Arguments { get; }

        public string Method { get; }

        public InvocationResult ReturnValue { get; }

        public DateTimeOffset Timestamp { get; }

        public InvocationLog(DateTimeOffset timestamp, object[] arguments, MethodInfo methodInfo, InvocationResult returnValue)
        {
            Arguments = arguments;
            Method = methodInfo.Name;
            ReturnValue = returnValue;
            Timestamp = timestamp;
        }
    }
}
