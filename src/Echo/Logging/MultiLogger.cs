using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Echo.Logging
{
    internal class MultiLogger : ILogger
    {
        private readonly ILogger[] _loggers;

        public MultiLogger(params ILogger[] loggers)
        {
            _loggers = loggers;
        }

        public void Debug(string message)
        {
            foreach (var logger in _loggers)
            {
                logger.Debug(message);
            }
        }

        public void Error(string message)
        {
            foreach (var logger in _loggers)
            {
                logger.Error(message);
            }
        }

        public void Fatal(string message)
        {
            foreach (var logger in _loggers)
            {
                logger.Fatal(message);
            }
        }

        public void Info(string message)
        {
            foreach (var logger in _loggers)
            {
                logger.Info(message);
            }
        }

        public void Warning(string message)
        {
            foreach (var logger in _loggers)
            {
                logger.Warning(message);
            }
        }
    }
}
