using System.Reflection;

namespace Echo.Core
{
    public interface IInvocationWriter
    {
        void WriteInvocation(MethodInfo methodInfo, InvocationResult invocationResult, object[] arguments);
    }
}
