using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using PodcastUtilities.AndroidLogic.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PodcastUtilities
{
    [Application]
    public class AndroidApplication : Application
    {
        public const string LOGCAT_TAG = "PodcastUtilities-Tag";

        public ILoggerFactory LoggerFactory { get; private set; }
        public ILogger Logger { get; private set; }
        public String DisplayVersion { get; private set; }

        // we must have a ctor or the app will not start
        protected AndroidApplication(System.IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        private void SetupExceptionHandler()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
            TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;
        }

        private void TaskSchedulerOnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs unobservedTaskExceptionEventArgs)
        {
            Logger.LogException(() => "TaskSchedulerOnUnobservedTaskException", unobservedTaskExceptionEventArgs.Exception);
        }

        private void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            Logger.LogException(() => "CurrentDomainOnUnhandledException", unhandledExceptionEventArgs.ExceptionObject as Exception);
        }

        public override void OnCreate()
        {
            Log.Debug(LOGCAT_TAG, $"AndroidApplication:OnCreate SDK == {Android.OS.Build.VERSION.SdkInt}, {(int)Android.OS.Build.VERSION.SdkInt}");
            Log.Debug(LOGCAT_TAG, $"AndroidApplication:OnCreate PackageName == {this.PackageName}");
            SetupExceptionHandler();
            var package = PackageManager.GetPackageInfo(PackageName, 0);
#if DEBUG
            var config = "(Debug)";
#else
            var config = "(Release)";
#endif
            DisplayVersion = $"v{package.VersionName}, ({package.VersionCode}), {config}";
            Log.Debug(LOGCAT_TAG, $"AndroidApplication:OnCreate Version == {DisplayVersion}");

            base.OnCreate();
            LoggerFactory = new NLoggerLoggerFactory();
            Logger = LoggerFactory.Logger;
            Logger.Debug(() => $"AndroidApplication:Logging init");

            Logger.Debug(() => $"AndroidApplication:IoC Init, {DisplayVersion}, {Android.OS.Build.VERSION.SdkInt}, {(int)Android.OS.Build.VERSION.SdkInt}, {this.PackageName}");
        }
    }
}