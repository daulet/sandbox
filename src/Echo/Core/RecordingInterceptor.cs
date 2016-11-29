using Castle.DynamicProxy;
using System;
using System.Threading.Tasks;

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
            try
            {
                invocation.Proceed();

                if (typeof(Task).IsAssignableFrom(invocation.Method.ReturnType))
                {
                    invocation.ReturnValue = InterceptAsync(invocation, (dynamic)invocation.ReturnValue);
                }
                else
                {
                    _tapeWritter.RecordInvocation(invocation.Method, new ValueInvocationResult(invocation.ReturnValue), invocation.Arguments);
                }
            }
            catch (Exception ex)
            {
                _tapeWritter.RecordInvocation(invocation.Method, new ExceptionInvocationResult(ex), invocation.Arguments);
            }
        }

        private async Task InterceptAsync(IInvocation invocation, Task task)
        {
            await task.ConfigureAwait(false);

            _tapeWritter.RecordInvocation(invocation.Method, InvocationResult.Void, invocation.Arguments);
        }

        private async Task<T> InterceptAsync<T>(IInvocation invocation, Task<T> task)
        {
            var result = await task.ConfigureAwait(false);

            _tapeWritter.RecordInvocation(invocation.Method, new ValueInvocationResult(result), invocation.Arguments);

            return result;
        }
    }
}
