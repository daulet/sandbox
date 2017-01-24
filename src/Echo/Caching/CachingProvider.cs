using System;
using Castle.DynamicProxy;
using Echo.Core;
using Echo.Restriction;

namespace Echo.Caching
{
    public class CachingProvider
    {
        private readonly ICacheProvider _cacheProvider;
        private readonly ProxyGenerator _generator = new ProxyGenerator();

        public CachingProvider(ICacheProvider cacheProvider)
        {
            _cacheProvider = cacheProvider;
        }

        public TTarget GetCachingTarget<TTarget>(TTarget target)
            where TTarget : class
        {
            // only public interface restricting is supported
            var targetType = typeof(TTarget);
            if (!targetType.IsInterface || !targetType.IsPublic)
            {
                throw new NotSupportedException();
            }

            var cachingInterceptor = new CachingInterceptor<TTarget>(_cacheProvider);
            return _generator.CreateInterfaceProxyWithTarget<TTarget>(target,
                new ListeningInterceptor<TTarget>(InstancePool.LoggingListener),
                cachingInterceptor);
        }
    }
}
