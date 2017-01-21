using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Echo.Logging
{
    internal class DebugLogger : ILogger
    {
        public void Debug(string message)
        {
            Console.WriteLine($"Debug: {message}");
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
            Console.WriteLine($"Info: {message}");
        }

        public void Warning(string message)
        {
            Console.WriteLine($"Warning: {message}");
        }
    }
}
