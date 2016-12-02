using Castle.DynamicProxy;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
                object returnValue = invocation.ReturnValue;
                if (invocation.Method.ReturnType != invocation.ReturnValue.GetType() && invocation.ReturnValue.GetType() == typeof(JObject))
                {
                    //var deserializeObjectMethods =
                    //    typeof(JToken)
                    //        .GetMethods(BindingFlags.Public).Where(x => x.Name == "ToObject" && x.IsGenericMethod).Where(method => method.GetParameters().Length == 0).First(m => m.GetParameters().First().ParameterType == typeof(string));

                    var deserializeObjectMethods =
                        typeof(JObject).GetMethods().Where(x => x.Name == "ToObject" && x.IsGenericMethod).First(method => method.GetParameters().Length == 0);

                    var genericDeserializerMethod = deserializeObjectMethods.MakeGenericMethod(invocation.Method.ReturnType);
                     returnValue = genericDeserializerMethod.Invoke(invocation.ReturnValue, null);
                }

                _invocationListener.WriteInvocation<TTarget>(invocation.Method, new ValueInvocationResult(returnValue),
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
