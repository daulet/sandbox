using Castle.DynamicProxy;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Echo.Core
{
    internal class ListeningInterceptor<TTarget> : IInterceptor
        where TTarget : class
    {
        private readonly IInvocationListener _invocationListener;

        internal ListeningInterceptor(IInvocationListener invocationListener)
        {
            _invocationListener = invocationListener;
        }

        public void Intercept(IInvocation invocation)
        {
            try
            {
                invocation.Proceed();
            }
            catch (Exception ex)
            {
                _invocationListener.WriteInvocation<TTarget>(invocation.Method, new ExceptionInvocationResult(ex), invocation.Arguments);

                throw;
            }

            if (typeof(void) == invocation.Method.ReturnType)
            {
                _invocationListener.WriteInvocation<TTarget>(invocation.Method, InvocationResult.Void, invocation.Arguments);
            }
            else if (typeof(Task).IsAssignableFrom(invocation.Method.ReturnType))
            {
                invocation.ReturnValue = InterceptAsync(invocation, (dynamic)invocation.ReturnValue);
            }
            else
            {
                _invocationListener.WriteInvocation<TTarget>(invocation.Method, new ValueInvocationResult(invocation.ReturnValue),
                    invocation.Arguments);
            }
        }

        private async Task InterceptAsync(IInvocation invocation, Task task)
        {
            try
            {
                await task.ConfigureAwait(false);

                _invocationListener.WriteInvocation<TTarget>(invocation.Method, InvocationResult.Void, invocation.Arguments);
            }
            catch (Exception ex)
            {
                _invocationListener.WriteInvocation<TTarget>(invocation.Method, new ExceptionInvocationResult(ex), invocation.Arguments);

                throw;
            }
        }

        private async Task<T> InterceptAsync<T>(IInvocation invocation, Task<T> task)
        {
            try
            {
                var result = await task.ConfigureAwait(false);

                _invocationListener.WriteInvocation<TTarget>(invocation.Method, new ValueInvocationResult(result), invocation.Arguments);

                return result;
            }
            catch (Exception ex)
            {
                _invocationListener.WriteInvocation<TTarget>(invocation.Method, new ExceptionInvocationResult(ex), invocation.Arguments);

                throw;
            }
        }
    }
}
