using System;

using Firebase.Crashlytics;
using PodcastUtilities.AndroidLogic.Exceptions;
using PodcastUtilities.Common.Platform;

namespace PodcastUtilities.AndroidLogic.Utilities
{
    public interface ICrashReporter
    {
        void TestReporting();
        void LogNonFatalException(Exception ex);
        void LogNonFatalException(string message, Exception ex);
    }

    public class NullCrashReporter : ICrashReporter
    {
        public void LogNonFatalException(Exception ex)
        {
        }

        public void LogNonFatalException(string message, Exception ex)
        {
        }

        public void TestReporting()
        {
        }
    }

    public class CrashlyticsReporter : ICrashReporter
    {
        // to see crashes go to
        // https://console.firebase.google.com/project/podcastutilities/crashlytics/app/android:com.andrewandderek.podcastutilities.sideload.debug

        private IAndroidEnvironmentInformationProvider AndroidEnvironmentInformationProvider;

        public CrashlyticsReporter(IAndroidEnvironmentInformationProvider androidEnvironmentInformationProvider)
        {
            AndroidEnvironmentInformationProvider = androidEnvironmentInformationProvider;

            var crashlytics = FirebaseCrashlytics.Instance;
            crashlytics.SetCustomKey("isWsa", AndroidEnvironmentInformationProvider.IsWsa());
            crashlytics.SetCustomKey("isKindle", AndroidEnvironmentInformationProvider.IsWsa());
        }

        public void LogNonFatalException(Exception ex)
        {
            var throwable = Java.Lang.Throwable.FromException(ex);
            FirebaseCrashlytics.Instance.RecordException(throwable);
        }

        public void LogNonFatalException(string message, Exception ex)
        {
            var wrappedException = new NonFatalMessageException(message, ex);
            var throwable = Java.Lang.Throwable.FromException(wrappedException);
            FirebaseCrashlytics.Instance.RecordException(throwable);
        }

        public void TestReporting()
        {
            throw new NotImplementedException("Test Crash Reporting");
        }
    }

    /* 
     * - cannot leave this code here as we have removed the NuGet package references
     *

    public class AppCenterCrashReporter : ICrashReporter
    {
        // to see crashes go to
        // https://appcenter.ms/orgs/AndrewAndDerek/apps/PodcastUtilitiesDebug/crashes/errors?appBuild=&period=last28Days&status=&version=

        private IAndroidEnvironmentInformationProvider AndroidEnvironmentInformationProvider;

        private Dictionary<string, string> LoggingExtraProperties = null;

        public AppCenterCrashReporter(IAndroidEnvironmentInformationProvider androidEnvironmentInformationProvider)
        {
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
        }

        public void LogNonFatalException(string message, Exception ex)
        {
            var properties = new Dictionary<string, string>(LoggingExtraProperties);
            properties.Add("message", message);
            Crashes.TrackError(ex, properties);
        }

        public void TestReporting()
        {
            // Note - this will only do anything in a debug build
            Crashes.GenerateTestCrash();
        }
    }

    */

}