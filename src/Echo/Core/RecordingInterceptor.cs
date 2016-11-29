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

                var method = invocation.MethodInvocationTarget;
                if (typeof(Task).IsAssignableFrom(method.ReturnType))
                {
                    invocation.ReturnValue = InterceptAsync(invocation, (dynamic)invocation.ReturnValue);
                }
                else
                {
                    _tapeWritter.RecordInvocation(invocation.Method, invocation.ReturnValue, invocation.Arguments);
                }
            }
            catch (Exception)
            {
                _tapeWritter.RecordInvocation(invocation.Method, invocation.ReturnValue, invocation.Arguments);
            }
        }

        private async Task InterceptAsync(IInvocation invocation, Task task)
        {
            await task.ConfigureAwait(false);

            _tapeWritter.RecordInvocation(invocation.Method, null, invocation.Arguments);
        }

        private async Task<T> InterceptAsync<T>(IInvocation invocation, Task<T> task)
        {
            var result = await task.ConfigureAwait(false);

            _tapeWritter.RecordInvocation(invocation.Method, result, invocation.Arguments);

            return result;
        }
    }
}
