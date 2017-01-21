using System;
using Castle.DynamicProxy;
using Echo.Core;
using Echo.Logging;

namespace Echo.Restriction
{
    public class RestrictingProvider
    {
        private readonly ProxyGenerator _generator = new ProxyGenerator();
        private readonly ILogger _logger;

        public RestrictingProvider(ILogger logger)
        {
            _logger = logger;
        }

        public TTarget GetRestrictingTarget<TTarget>(TTarget target)
            where TTarget : class
        {
            // only public interface restricting is supported
            var targetType = typeof(TTarget);
            if (!targetType.IsInterface || !targetType.IsPublic)
            {
                throw new NotSupportedException();
            }

            var recordingInterceptor = new RestrictingInterceptor<TTarget>(_logger);
            return _generator.CreateInterfaceProxyWithTarget<TTarget>(target,
                new ListeningInterceptor<TTarget>(InstancePool.LoggingListener),
                recordingInterceptor);
        }
    }
}
