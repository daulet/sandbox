using Castle.DynamicProxy;
using Echo.Logging;
using System;
using System.Linq;

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
            if (invocation.Method.CustomAttributes.Any(x => x.AttributeType == typeof(RestrictedAttribute)))
            {
                _logger.Info($"Restricting call to {invocation.Method.Name} with {string.Join(", ", invocation.Arguments)}");

                var returnType = invocation.Method.ReturnType;
                if (returnType != typeof(void) && returnType.IsValueType)
                {
                    invocation.ReturnValue = Activator.CreateInstance(returnType);
                }
            }
            else
            {
                invocation.Proceed();
            }
        }
    }
}
