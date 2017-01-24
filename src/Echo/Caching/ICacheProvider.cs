using System;

namespace Echo.Caching
{
    public interface ICacheProvider
    {
        string GetValue(string key);

        void SetValue(string key, string value, TimeSpan expiration);
    }
}
