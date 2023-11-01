using Microsoft.AppCenter.Crashes;
using System;
using System.Collections.Generic;

namespace PodcastUtilities.AndroidLogic.Utilities
{
    public interface ICrashReporter
    {
        void TestReporting();
        void LogNonFatalException(Exception ex);
    }

    public class AppCenterCrashReporter : ICrashReporter
    {
        // to see crashes go to
        // https://appcenter.ms/orgs/AndrewAndDerek/apps/PodcastUtilitiesDebug/crashes/errors?appBuild=&period=last28Days&status=&version=

        private IAnalyticsEngine AnalyticsEngine;
        private IAndroidEnvironmentInformationProvider AndroidEnvironmentInformationProvider;

        private Dictionary<string, string> LoggingExtraProperties = null;

        public AppCenterCrashReporter(IAnalyticsEngine analyticsEngine, IAndroidEnvironmentInformationProvider androidEnvironmentInformationProvider)
        {
            AnalyticsEngine = analyticsEngine;
            AndroidEnvironmentInformationProvider = androidEnvironmentInformationProvider;

            LoggingExtraProperties = new Dictionary<string, string>
            {
                { "IsKindle", AndroidEnvironmentInformationProvider.IsKindleFire().ToString() },
                { "IsWsa", AndroidEnvironmentInformationProvider.IsWsa().ToString()}
            };
    }

    public void LogNonFatalException(Exception ex)
        {
            Crashes.TrackError(ex, LoggingExtraProperties);
            AnalyticsEngine?.LifecycleErrorEvent();
        }

        public void TestReporting()
        {
            // Note - this will only do anything in a debug build
            Crashes.GenerateTestCrash();
        }
    }


    /* 
     * - cannot leave this code here as we have removed the NuGet package references
     *

    public class CrashlyticsReporter : ICrashReporter
    {
        // to see crashes go to
        // https://console.firebase.google.com/project/podcastutilities/crashlytics/app/android:com.andrewandderek.podcastutilities.sideload.debug

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

    */

}