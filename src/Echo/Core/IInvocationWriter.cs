using System.Reflection;

namespace Echo.Core
{
    // TODO rename InvocationListener
    public interface IInvocationWriter
    {
        void WriteInvocation<TTarget>(MethodInfo methodInfo, InvocationResult invocationResult, object[] arguments)
            where TTarget : class;
    }
}
