using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Echo.Logging
{
    internal class ReleaseLogger : ILogger
    {
        public void Debug(string message)
        {
            // not logging debug in release mode
        }

        public void Error(string message)
        {
            Console.WriteLine($"Error: {message}");
        }

        public void Fatal(string message)
        {
            Console.WriteLine($"Fatal: {message}");
        }

        public void Info(string message)
        {
            // not logging info in release mode
        }

        public void Warning(string message)
        {
            Console.WriteLine($"Warning: {message}");
        }
    }
}
