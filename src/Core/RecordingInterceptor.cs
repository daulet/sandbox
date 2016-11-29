using Castle.DynamicProxy;

namespace Echo.Core
{
    internal class RecordingInterceptor : IInterceptor
    {
        private readonly IInvocationWritter _tapeWritter;

        public RecordingInterceptor(IInvocationWritter tapeWritter)
        {
            _tapeWritter = tapeWritter;
        }

        public void Intercept(IInvocation invocation)
        {
            invocation.Proceed();

            _tapeWritter.RecordInvocation(invocation.Method, invocation.ReturnValue, invocation.Arguments);
        }
    }
}
