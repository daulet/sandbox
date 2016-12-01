using Echo.Core;
using System;
using System.Reflection;

namespace Echo.UnitTesting
{
    internal class TestInvocation
    {
        public object[] Arguments { get; }

        public MethodInfo Method { get; }

        public InvocationResult Result { get; }

        public Type TargetType { get; }

        public TestInvocation(object[] arguments, MethodInfo method, InvocationResult result, Type targetType)
        {
            Arguments = arguments;
            Method = method;
            Result = result;
            TargetType = targetType;
        }
    }
}
