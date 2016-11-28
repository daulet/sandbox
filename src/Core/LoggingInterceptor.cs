using Castle.DynamicProxy;

namespace Echo.Core
{
    internal class LoggingInterceptor : IInterceptor
    {
        private readonly ITapeWritter _tapeWritter;

        public LoggingInterceptor(ITapeWritter tapeWritter)
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
