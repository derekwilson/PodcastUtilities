using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.IO;
using System.Xml;

namespace PodcastUtilities.AndroidLogic.Logging
{
    public interface ILogger
    {
        public delegate string MessageGenerator();

        void Debug(MessageGenerator message);
        void Warning(MessageGenerator message);
        void LogException(MessageGenerator message, Exception ex);
    }
    public class NLogLogger : ILogger
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

    public class NLoggerLoggerFactory : ILoggerFactory
    {
        /// <summary>
        /// Use whatever is in the config file
        /// </summary>
        public NLoggerLoggerFactory()
        {
        }

        /// <summary>
        /// setup the file target with the supplied folder
        /// </summary>
        /// <param name="folder">folder for log files</param>
        public NLoggerLoggerFactory(String folder)
        {
            var config = LogManager.Configuration;
            var target = config.FindTargetByName("externalFileTarget");
            var fileTarget = target as FileTarget;
            fileTarget.FileName = Path.Combine(folder, "logs/${shortdate}.log.csv");
            fileTarget.ArchiveFileName = Path.Combine(folder, "logs/archive.{#}.log.csv");
            LogManager.ReconfigExistingLoggers();
        }

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