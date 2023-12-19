using PodcastUtilities.AndroidLogic.Logging;
using System;

namespace PodcastUtilities.AndroidLogic.Utilities
{
    public interface IValueConverter
    {
        string ConvertToString(long value);
        string ConvertToString(int value);
        long ConvertStringToLong(string value);
        int ConvertStringToInt(string value);
    }
    public class ValueConverter : IValueConverter
    {
        private ICrashReporter CrashReporter;
        private ILogger Logger;

        public ValueConverter(
            ICrashReporter crashReporter,
            ILogger logger
        )
        {
            CrashReporter = crashReporter;
            Logger = logger;
        }

        public int ConvertStringToInt(string value)
        {
            try
            {
                return int.Parse(value);
            }
            catch (Exception ex)
            {
                CrashReporter.LogNonFatalException(ex);
                Logger.LogException(() => "error converting number", ex);
                return 0;
            }
        }

        public long ConvertStringToLong(string value)
        {
            try
            {
                return long.Parse(value);
            } catch(Exception ex)
            {
                CrashReporter.LogNonFatalException(ex);
                Logger.LogException(() => "error converting number", ex);
                return 0;
            }
        }

        public string ConvertToString(long value)
        {
            return value.ToString();
        }

        public string ConvertToString(int value)
        {
            return value.ToString();
        }
    }
}