using System.Reflection;

namespace Echo.Core
{
    internal interface IInvocationWriter
    {
        void WriteInvocation(MethodInfo methodInfo, InvocationResult invocationResult, object[] arguments);
    }
}
