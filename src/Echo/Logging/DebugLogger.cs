using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Echo.Logging
{
    internal class DebugLogger : ILogger
    {
        public void Error(string message)
        {
            Console.WriteLine($"Error: {message}");
        }

        public void Info(string message)
        {
            Console.WriteLine($"Info: {message}");
        }

        public void Verbose(string message)
        {
            Console.WriteLine($"Verbose: {message}");
        }

        public void Warning(string message)
        {
            Console.WriteLine($"Warning: {message}");
        }
    }
}
