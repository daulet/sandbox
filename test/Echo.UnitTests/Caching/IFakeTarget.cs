using Echo.Caching;

namespace Echo.UnitTests.Caching
{
    public interface IFakeTarget
    {
        object NotCachedMethod();

        [Fake]
        object NotCachedMethodWithCustomAttribute();

        [Cached(1)]
        object CachedMethod();

        [Fake]
        [Cached(1)]
        object CachedMethodWithCustomAttribute();
    }
}
