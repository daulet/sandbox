using System;
using System.Reflection;

namespace Echo.UnitTesting
{
    internal class TestInvocation
    {
        public object[] Arguments { get; }

        public MethodInfo Method { get; }

        public Type TargetType { get; }

        public TestInvocation(object[] arguments, MethodInfo method, Type targetType)
        {
            Arguments = arguments;
            Method = method;
            TargetType = targetType;
        }
    }
}
