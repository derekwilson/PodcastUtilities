using Firebase.Crashlytics;
using System;

namespace PodcastUtilities.AndroidLogic.Utilities
{
    public interface ICrashReporter
    {
        void TestReporting();
        void LogNonFatalException(Exception ex);
    }

    public class CrashlyticsReporter : ICrashReporter
    {
        private IAnalyticsEngine AnalyticsEngine;

        public CrashlyticsReporter(IAnalyticsEngine analyticsEngine)
        {
            AnalyticsEngine = analyticsEngine;
        }

        public void LogNonFatalException(Exception ex)
        {
            var throwable = Java.Lang.Throwable.FromException(ex);
            FirebaseCrashlytics.Instance.RecordException(throwable);

            AnalyticsEngine?.LifecycleErrorEvent();
        }

        public void TestReporting()
        {
            throw new NotImplementedException("Test Crash Reporting");
        }
    }
}