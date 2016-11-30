using System.Reflection;

namespace Echo.Core
{
    internal interface IInvocationReader
    {
        InvocationResult FindInvocationResult(MethodInfo methodInfo, object[] arguments);
    }
}
