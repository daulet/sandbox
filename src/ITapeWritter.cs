using System.Reflection;

namespace Echo
{
    public interface ITapeWritter
    {
        void RecordInvocation(MethodInfo methodInfo, object returnValue, object[] arguments);
    }
}
