using System.Reflection;

namespace Echo
{
    public interface IInvocationReader
    {
        object ReadReturnValue(MethodInfo methodInfo, object[] arguments);
    }
}
