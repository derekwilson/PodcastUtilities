using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PodcastUtilitiesPOC
{
    public interface ILogger
    {
        public delegate string MessageGenerator();

        void Debug(MessageGenerator message);
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