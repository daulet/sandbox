using Castle.DynamicProxy;
using Echo.Logging;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Echo.Restriction
{
    internal class RestrictingInterceptor : IInterceptor
    {
        private readonly ILogger _logger;

        public RestrictingInterceptor(ILogger logger)
        {
            _logger = logger;
        }

        public void Intercept(IInvocation invocation)
        {
            var attribute = invocation.Method
                .GetCustomAttributes(typeof(RestrictedAttribute), false)
                .Cast<RestrictedAttribute>()
                .FirstOrDefault();
            if (attribute != null)
            {
                _logger.Info($"Restricting call to {invocation.Method.Name} with {string.Join(", ", invocation.Arguments)}");

                var returnType = invocation.Method.ReturnType;

                if (returnType == typeof(void))
                {
                    return;
                }

                if (attribute.ProviderType != null)
                {
                    if (Activator.CreateInstance(attribute.ProviderType) is IReturnValueProvider valueProvider)
                    {
                        invocation.ReturnValue = valueProvider.GetReturnValue(returnType);
                        return;
                    }
                    else
                    {
                        _logger.Info($"Provided {attribute.ProviderType} does not implement IReturnValueProvider");
                    }
                }

                if (typeof(Task).IsAssignableFrom(returnType))
                {
                    if (returnType.IsGenericType) // Task<TResult>
                    {
                        invocation.ReturnValue = GetType()
                            .GetMethod("GetDefaultCompletedTask",
                                BindingFlags.Instance | BindingFlags.NonPublic)
                            .MakeGenericMethod(returnType.GenericTypeArguments[0])
                            .Invoke(this, null);
                    }
                    else // Task
                    {
                        invocation.ReturnValue = Task.FromResult(0);
                    }
                }
                else if (returnType.IsValueType) // scalar
                {
                    invocation.ReturnValue = Activator.CreateInstance(returnType);
                }
            }
            else
            {
                invocation.Proceed();
            }
        }

        private Task<T> GetDefaultCompletedTask<T>()
        {
            return Task.FromResult(default(T));
        }
    }
}
