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
            }
            catch (Exception ex)
            {
                _tapeWritter.RecordInvocation(invocation.Method, new ExceptionInvocationResult(ex), invocation.Arguments);

                throw;
            }

            if (typeof(void) == invocation.Method.ReturnType)
            {
                _tapeWritter.RecordInvocation(invocation.Method, InvocationResult.Void, invocation.Arguments);
            }
            else if (typeof(Task).IsAssignableFrom(invocation.Method.ReturnType))
            {
                invocation.ReturnValue = InterceptAsync(invocation, (dynamic)invocation.ReturnValue);
            }
            else
            {
                _tapeWritter.RecordInvocation(invocation.Method, new ValueInvocationResult(invocation.ReturnValue),
                    invocation.Arguments);
            }

        }

        private async Task InterceptAsync(IInvocation invocation, Task task)
        {
            try
            {
                await task.ConfigureAwait(false);

                _tapeWritter.RecordInvocation(invocation.Method, InvocationResult.Void, invocation.Arguments);
            }
            catch (Exception ex)
            {
                _tapeWritter.RecordInvocation(invocation.Method, new ExceptionInvocationResult(ex), invocation.Arguments);

                throw;
            }
        }

        private async Task<T> InterceptAsync<T>(IInvocation invocation, Task<T> task)
        {
            try
            {
                var result = await task.ConfigureAwait(false);

                _tapeWritter.RecordInvocation(invocation.Method, new ValueInvocationResult(result), invocation.Arguments);

                return result;
            }
            catch (Exception ex)
            {
                _tapeWritter.RecordInvocation(invocation.Method, new ExceptionInvocationResult(ex), invocation.Arguments);

                throw;
            }
        }
    }
}
