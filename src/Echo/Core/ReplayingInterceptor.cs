using Castle.DynamicProxy;

namespace Echo.Core
{
    internal class ReplayingInterceptor : IInterceptor
    {
        private readonly IInvocationReader _invocationReader;

        internal ReplayingInterceptor(IInvocationReader invocationReader)
        {
            _invocationReader = invocationReader;
        }

        public void Intercept(IInvocation invocation)
        {
            var returnValue = _invocationReader.ReadReturnValue(invocation.Method, invocation.Arguments);
            invocation.ReturnValue = returnValue;
        }
    }
}
