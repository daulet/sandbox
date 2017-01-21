using Echo.Logging;

namespace Echo.Core
{
    internal static class InstancePool
    {
        private static ILogger Logger { get; } =
#if DEBUG
            new DebugLogger();
#else
            new ReleaseLogger();
#endif

        public static IInvocationListener LoggingListener { get; } = new LoggingListener(Logger);
    }
}
