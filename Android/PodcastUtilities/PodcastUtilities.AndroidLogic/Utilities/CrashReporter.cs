using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Firebase.Crashlytics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PodcastUtilities.AndroidLogic.Utilities
{
    public interface ICrashReporter
    {
        void testReporting();
        void logNonFatalException(Exception ex);
    }

    public class CrashReporter : ICrashReporter
    {
        public void logNonFatalException(Exception ex)
        {
            var throwable = Java.Lang.Throwable.FromException(ex);
            FirebaseCrashlytics.Instance.RecordException(throwable);
        }

        public void testReporting()
        {
            throw new NotImplementedException("Test Crash Reporting");
        }
    }
}