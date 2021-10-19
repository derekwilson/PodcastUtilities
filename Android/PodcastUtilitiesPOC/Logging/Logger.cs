using NLog;
using System;

namespace PodcastUtilitiesPOC.Logging
{
    public interface ILogger
    {
        public delegate string MessageGenerator();

        void Debug(MessageGenerator message);
        void Warning(MessageGenerator message);
        void LogException(MessageGenerator message, Exception ex);
    }
    class NLogLogger : ILogger
    {
        private Logger nlogLogger;

        public NLogLogger(Logger logger)
        {
            nlogLogger = logger;
        }

        public void Debug(ILogger.MessageGenerator message)
        {
            if (nlogLogger.IsEnabled(LogLevel.Debug))
            {
                // only call the message delegate if we are logging
                nlogLogger.Debug(message());
            }
        }

        public void Warning(ILogger.MessageGenerator message)
        {
            if (nlogLogger.IsEnabled(LogLevel.Warn))
            {
                // only call the message delegate if we are logging
                nlogLogger.Warn(message());
            }
        }

        public void LogException(ILogger.MessageGenerator message, Exception ex)
        {
            nlogLogger.Error(ex, message() + $" => {ex.Message}");
        }
    }

    public interface ILoggerFactory
    {
        ILogger Logger { get; }
    }

    class NLoggerLoggerFactory : ILoggerFactory
    {
        public ILogger Logger
        {
            get
            {
                var logger = LogManager.GetCurrentClassLogger();
                return new NLogLogger(logger);
            }
        }
    }
}