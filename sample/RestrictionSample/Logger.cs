using System;
using Echo.Logging;

namespace Echo.Sample.RestrictionSample
{
    public class Logger : ILogger
    {
        public void Verbose(string message)
        {
            Console.WriteLine(message);
        }

        public void Error(string message)
        {
            Console.WriteLine(message);
        }

        public void Info(string message)
        {
            Console.WriteLine(message);
        }

        public void Warning(string message)
        {
            Console.WriteLine(message);
        }
    }
}
