using Castle.DynamicProxy;
using System;

namespace Echo.Core
{
    internal class ReplayingInterceptor<TTarget> : IInterceptor
        where TTarget : class
    {
        private readonly IInvocationReader _invocationReader;

        internal ReplayingInterceptor(IInvocationReader invocationReader)
        {
            _invocationReader = invocationReader;
        }

        // TODO doesn't handle async results
        public void Intercept(IInvocation invocation)
        {
            try
            {
                var returnValue = _invocationReader.FindInvocationResult<TTarget>(invocation.Method, invocation.Arguments);
                if (returnValue is ExceptionInvocationResult)
                {
                    throw (returnValue as ExceptionInvocationResult).ThrownException;
                }
                else if (returnValue is ValueInvocationResult)
                {
                    invocation.ReturnValue = (returnValue as ValueInvocationResult).ReturnedValue;
                }
                else if (returnValue is VoidInvocationResult)
                {
                    // no ReturnValue in this case
                }
                else
                {
                    throw new NotSupportedException();
                }
            }
            catch (NoEchoFoundException)
            {
                // TODO This behaviour needs to be configurable: return null or throw
                throw;
            }
        }
    }
}
