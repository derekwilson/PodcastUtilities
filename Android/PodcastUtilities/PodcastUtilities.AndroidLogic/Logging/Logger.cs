using Android.Content.Res;
using NLog;
using NLog.Targets;
using System;
using System.IO;
using System.Net.Security;
using System.Reflection;
using static Android.Telephony.CarrierConfigManager;

namespace PodcastUtilities.AndroidLogic.Logging
{
    public interface ILogger
    {
        public delegate string MessageGenerator();

        void Debug(MessageGenerator message);
        void Warning(MessageGenerator message);
        void LogException(MessageGenerator message, Exception ex);
    }
    
    /// <summary>
    /// An implementation of ILogger that uses NLog
    /// </summary>
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

    /// <summary>
    /// An implementation of the ILoggerFactory that generates an NlogLogger
    /// </summary>
    public class NLoggerLoggerFactory : ILoggerFactory
    {
        private void LoadConfig(AssetManager assets)
        {
            // NLog 5 removed Xamarin specific builds so the automatic loading of config was removed
            // see https://nlog-project.org/2021/08/25/nlog-5-0-preview1-ready.html

            using (Stream inputStream = assets.Open("NLog.config")) 
            {
                using (var xmlReader = System.Xml.XmlReader.Create(inputStream))
                {
                    LogManager.Configuration = new NLog.Config.XmlLoggingConfiguration(xmlReader, null);
                }
            }
        }

        /// <summary>
        /// Use whatever is in the config file
        /// </summary>
        public NLoggerLoggerFactory(AssetManager assets)
        {
            LoadConfig(assets);
        }

        /// <summary>
        /// setup the file target with the supplied folder
        /// </summary>
        /// <param name="folder">folder for log files</param>
        public NLoggerLoggerFactory(AssetManager assets, String folder)
        {
            LoadConfig(assets);
            // set the targets for the file loggers
            var config = LogManager.Configuration;
            var target = config.FindTargetByName("externalFileTarget");
            var fileTarget = target as FileTarget;
            fileTarget.FileName = Path.Combine(folder, "logs/${shortdate}.log.csv");
            fileTarget.ArchiveFileName = Path.Combine(folder, "logs/archive.{#}.log.csv");

            // set the loglevel
#if DEBUG
            SetLoggingLevel(LogLevel.Debug);
#else
            SetLoggingLevel(LogLevel.Error);
#endif
            // re-apply the config
            LogManager.ReconfigExistingLoggers();
        }

        private void SetLoggingLevel(LogLevel minLevel)
        {
            if (minLevel == LogLevel.Off)
            {
                LogManager.SuspendLogging();
                return;
            }

            if (!LogManager.IsLoggingEnabled())
            {
                LogManager.ResumeLogging();
            }
            foreach (var rule in LogManager.Configuration.LoggingRules)
            {
                rule.SetLoggingLevels(minLevel, LogLevel.Fatal);
            }
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