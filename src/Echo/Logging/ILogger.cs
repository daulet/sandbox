namespace Echo.Logging
{
    public interface ILogger
    {
        /// <summary>
        /// Logs a debug message.
        /// </summary>
        /// <param name="message">The message to log</param>
        void Debug(string message);

        /// <summary>
        /// Logs an error message.
        /// </summary>
        /// <param name="message">The message to log</param>
        void Error(string message);

        /// <summary>
        /// Logs a fatal message.
        /// </summary>
        /// <param name="message">The message to log</param>
        void Fatal(string message);

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