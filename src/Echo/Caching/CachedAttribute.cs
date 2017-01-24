using System;

namespace Echo.Caching
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
    public class CachedAttribute : Attribute
    {
        internal TimeSpan Expiration { get; }

        public CachedAttribute(int cacheExpirationInSeconds)
        {
            Expiration = TimeSpan.FromSeconds(cacheExpirationInSeconds);
        }
    }
}
