using System;
using System.Reflection;

namespace Echo.Core
{
    internal class Invocation
    {
        public object[] Arguments { get; }

        public MethodInfo Method { get; }

        public InvocationResult Result { get; }

        public Type Target { get; }

        public Invocation(object[] arguments, MethodInfo method, InvocationResult result, Type target)
        {
            Arguments = arguments;
            Method = method;
            Result = result;
            Target = target;
        }
    }
}
