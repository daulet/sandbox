using System;

namespace Echo.Logging
{
    internal class ReleaseLogger : ILogger
    {
        public void Error(string message)
        {
            Console.WriteLine($"Error: {message}");
        }

        public void Info(string message)
        {
            // not logging info in release mode
        }

        public void Verbose(string message)
        {
            // not logging debug in release mode
        }

        public void Warning(string message)
        {
            Console.WriteLine($"Warning: {message}");
        }
    }
}
