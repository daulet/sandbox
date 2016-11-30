using System.Reflection;

namespace Echo
{
    public interface IInvocationWritter
    {
        void WriteInvocation(MethodInfo methodInfo, InvocationResult invocationResult, object[] arguments);
    }
}
