using System.Reflection;
using Echo.Logging;

namespace Echo.Core
{
    internal class LoggingListener : IInvocationListener
    {
        private readonly ILogger _logger;

        public LoggingListener(ILogger logger)
        {
            _logger = logger;
        }

        // TODO improve logging of custom types => right now only printing type names
        public void WriteInvocation<TTarget>(MethodInfo methodInfo, InvocationResult invocationResult, object[] arguments)
            where TTarget : class
        {
            _logger.Debug($"Intercepting {typeof(TTarget).Name}.{methodInfo.Name} method:");
            foreach (var argument in arguments)
            {
                _logger.Debug($"\tArgument {argument?.GetType()}: {argument}");
            }
            _logger.Debug($"\tReturn {invocationResult?.GetResultType()}: {invocationResult}");
        }
    }
}
