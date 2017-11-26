using System;
using System.Reflection;
using Echo.Logging;
using Newtonsoft.Json;

namespace Echo.Core
{
    // @TODO this logging could cause confusing error messages if fails,
    // e.g. null reference thrown out of WriteInvocation.
    // Hence disable in Release version
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
            _logger.Verbose($"Intercepting {typeof(TTarget).Name}.{methodInfo.Name} method:");
            var parameters = methodInfo.GetParameters();
            for (var paramIndex = 0; paramIndex < parameters.Length; paramIndex++)
            {
                var argument = arguments[paramIndex];
                var serializedArgument = JsonConvert.SerializeObject(argument, Formatting.Indented);
                _logger.Verbose($"\tArgument '{parameters[paramIndex].Name}' ({parameters[paramIndex].ParameterType.Name}): " +
                              $"{serializedArgument}");
            }

            if (invocationResult is ExceptionInvocationResult)
            {
                _logger.Verbose($"\tThrew ({methodInfo.ReturnType.Name}): " +
                              $"{(invocationResult as ExceptionInvocationResult).ThrownException.GetType()}");
            }
            else if (invocationResult is ValueInvocationResult)
            {
                var serializedResult = JsonConvert.SerializeObject((invocationResult as ValueInvocationResult).ReturnedValue, Formatting.Indented);
                _logger.Verbose($"\tReturned ({methodInfo.ReturnType.Name}): " +
                              $"{serializedResult}");
            }
            else if (invocationResult is VoidInvocationResult)
            {
                _logger.Verbose("\tReturned (void)");
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
