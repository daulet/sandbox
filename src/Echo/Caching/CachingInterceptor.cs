using Castle.DynamicProxy;
using Newtonsoft.Json;
using System;
using System.Reflection;

namespace Echo.Caching
{
    internal class CachingInterceptor<TTarget> : IInterceptor
        where TTarget : class
    {
        private readonly ICacheProvider _cacheProvider;

        public CachingInterceptor(ICacheProvider cacheProvider)
        {
            _cacheProvider = cacheProvider;
        }

        public void Intercept(IInvocation invocation)
        {
            var cachedAttribute = invocation.Method.GetCustomAttribute<CachedAttribute>();
            if (cachedAttribute != null)
            {
                Intercept(invocation, cachedAttribute);
            }
            else
            {
                invocation.Proceed();
            }
        }

        // TODO check if return type is void
        private void Intercept(IInvocation invocation, CachedAttribute attribute)
        {
            var cacheKey = GetCacheKey(typeof(TTarget), invocation.Method, invocation.Arguments);
            var serializedCachedValue = _cacheProvider.GetValue(cacheKey);
            if (!string.IsNullOrEmpty(serializedCachedValue))
            {
                var cachedValue = JsonConvert.DeserializeObject(serializedCachedValue, invocation.Method.ReturnType);
                // TODO what if we cache null?
                if (cachedValue != null)
                {
                    invocation.ReturnValue = cachedValue;
                    return;
                }
            }

            invocation.Proceed();

            var actualValue = invocation.ReturnValue;
            var serializedActualValue = JsonConvert.SerializeObject(actualValue);
            _cacheProvider.SetValue(cacheKey, serializedActualValue, attribute.Expiration);
        }

        private static string GetCacheKey(Type targetType, MethodInfo methodInfo, object[] arguments)
        {
            int hash = 13;
            hash = (hash * 7) + targetType.GetHashCode();
            hash = (hash * 7) + methodInfo.GetHashCode();
            foreach (var argument in arguments)
            {
                if (argument == null)
                {
                    hash = (hash * 7) + 0.GetHashCode();
                }
                else
                {
                    hash = (hash * 7) + argument.GetHashCode();
                }
            }
            return hash.ToString();
        }
    }
}
