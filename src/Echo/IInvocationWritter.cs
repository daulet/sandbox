using System.Reflection;

namespace Echo
{
    public interface IInvocationWritter
    {
        void RecordInvocation(MethodInfo methodInfo, object returnValue, object[] arguments);
    }
}
