using System;
using System.Reflection;
using Echo.Logging;
using Newtonsoft.Json;

namespace Echo.Core
{
    internal class LoggingListener : IInvocationListener
    {
        private readonly ILogger _logger;

        public LoggingListener(ILogger logger)
        {
            _logger = logger;
        }

        public void WriteInvocation<TTarget>(MethodInfo methodInfo, InvocationResult invocationResult, object[] arguments)
            where TTarget : class
        {
            _logger.Debug($"Intercepting {typeof(TTarget).Name}.{methodInfo.Name} method:");
            var parameters = methodInfo.GetParameters();
            for (var paramIndex = 0; paramIndex < parameters.Length; paramIndex++)
            {
                var argument = arguments[paramIndex];
                var serializedArgument = JsonConvert.SerializeObject(argument, Formatting.Indented);
                _logger.Debug($"\tArgument '{parameters[paramIndex].Name}' ({parameters[paramIndex].ParameterType.Name}): " +
                              $"{serializedArgument}");
            }

            if (invocationResult is ExceptionInvocationResult)
            {
                _logger.Debug($"\tThrew ({methodInfo.ReturnType.Name}): " +
                              $"{(invocationResult as ExceptionInvocationResult).ThrownException.GetType()}");
            }
            else if (invocationResult is ValueInvocationResult)
            {
                var serializedResult = JsonConvert.SerializeObject((invocationResult as ValueInvocationResult).ReturnedValue, Formatting.Indented);
                _logger.Debug($"\tReturned ({methodInfo.ReturnType.Name}): " +
                              $"{serializedResult}");
            }
            else if (invocationResult is VoidInvocationResult)
            {
                _logger.Debug("\tReturned (void)");
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
