using Echo.Logging;

namespace Echo.Core
{
    internal static class InstancePool
    {
        private static ILogger Logger { get; } =
            new ReleaseLogger();

        public static IInvocationListener LoggingListener { get; } = new LoggingListener(Logger);
    }
}
