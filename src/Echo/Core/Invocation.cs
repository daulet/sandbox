using System;

namespace Echo.Core
{
    public class Invocation
    {
        public object[] Arguments { get; }

        public string Method { get; }

        public InvocationResult Result { get; }

        public Type Target { get; }

        public Invocation(object[] arguments, string method, InvocationResult result, Type target)
        {
            Arguments = arguments;
            Method = method;
            Result = result;
            Target = target;
        }
    }
}
