using System.Reflection;

namespace Echo
{
    public interface IInvocationReader
    {
        InvocationResult FindReturnValue(MethodInfo methodInfo, object[] arguments);
    }
}
