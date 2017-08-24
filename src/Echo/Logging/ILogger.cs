namespace Echo.Logging
{
    public interface ILogger
    {
        /// <summary>
        /// Logs a verbose message.
        /// </summary>
        /// <param name="message">The message to log</param>
        void Verbose(string message);

        /// <summary>
        /// Logs an error message.
        /// </summary>
        /// <param name="message">The message to log</param>
        void Error(string message);

        /// <summary>
        /// Logs an info message.
        /// </summary>
        /// <param name="message">The message to log</param>
        void Info(string message);

        /// <summary>
        /// Logs a warning message.
        /// </summary>
        /// <param name="message">The message to log</param>
        void Warning(string message);
    }
}
