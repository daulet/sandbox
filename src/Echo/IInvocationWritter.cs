using System.Reflection;

namespace Echo
{
    public interface IInvocationWritter
    {
        void RecordInvocation(MethodInfo methodInfo, InvocationResult invocationResult, object[] arguments);
    }
}
