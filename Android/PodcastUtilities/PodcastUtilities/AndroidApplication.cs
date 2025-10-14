using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Util;
using AndroidX.Core.Content.PM;
using PodcastUtilities.AndroidLogic.Converter;
using PodcastUtilities.AndroidLogic.CustomViews;
using PodcastUtilities.AndroidLogic.Logging;
using PodcastUtilities.AndroidLogic.MessageStore;
using PodcastUtilities.AndroidLogic.Services.Download;
using PodcastUtilities.AndroidLogic.Settings;
using PodcastUtilities.AndroidLogic.Utilities;
using PodcastUtilities.AndroidLogic.ViewModel;
using PodcastUtilities.AndroidLogic.ViewModel.Configure;
using PodcastUtilities.AndroidLogic.ViewModel.Download;
using PodcastUtilities.AndroidLogic.ViewModel.Help;
using PodcastUtilities.AndroidLogic.ViewModel.Main;
using PodcastUtilities.AndroidLogic.ViewModel.Messages;
using PodcastUtilities.AndroidLogic.ViewModel.Purge;
using PodcastUtilities.AndroidLogic.ViewModel.Settings;
using PodcastUtilities.Common;
using PodcastUtilities.Common.Platform;
using PodcastUtilities.Ioc;
using PodcastUtilities.Services.Download;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace PodcastUtilities
{
    [Application]
    public class AndroidApplication : Application, IAndroidApplication
    {
        // tag used in LogCat, only used until NLog starts running
        public const string LOGCAT_TAG = "PodcastUtilitiesTag";

        public NLoggerLoggerFactory? LoggerFactory { get; private set; }
        public ILogger? Logger { get; private set; }
        public String DisplayVersion { get; private set; }
        public String DisplayPackage { get; private set; }
        public IIocContainer? IocContainer { get; private set; }

        private IAnalyticsEngine? Analytics;

        // we must have a ctor or the app will not start
        protected AndroidApplication(System.IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
            DisplayVersion = "NOT SET";
            DisplayPackage = "NOT SET";
        }

        private void SetupExceptionHandler()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
            TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;
        }

        private void TaskSchedulerOnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs unobservedTaskExceptionEventArgs)
        {
            Logger?.LogException(() => "TaskSchedulerOnUnobservedTaskException", unobservedTaskExceptionEventArgs.Exception);
            Analytics?.LifecycleErrorFatalEvent();
        }

        private void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            var ex = unhandledExceptionEventArgs.ExceptionObject as Exception ?? new Exception("EXCEPTION NOT PROVIDED");
            Logger?.LogException(() => "CurrentDomainOnUnhandledException", ex);
            Analytics?.LifecycleErrorFatalEvent();
        }

        /// <summary>
        /// setup the core PodcastUtilities.Common components
        /// </summary>
        /// <returns>an initialised container</returns>
        private static IIocContainer InitializeIocContainer()
        {
            var container = IocRegistration.GetEmptyContainer();

            IocRegistration.RegisterSystemServices(container);
            IocRegistration.RegisterPortableDeviceServices(container);
            IocRegistration.RegisterFileServices(container);
            IocRegistration.RegisterFeedServices(container);
            IocRegistration.RegisterPlaylistServices(container);

            return container;
        }

        /// <summary>
        /// add extra android things to the container so we can inject our viewmodels
        /// </summary>
        /// <param name="container">container to add to</param>
        /// <returns>the updated container</returns>
        private IIocContainer? AddExtrasToIocContainer(IIocContainer container)
        {
            // the container itself, for factories
            container?.Register<IIocContainer>(container);
            // android things
            container?.Register<Context>(ApplicationContext!);
            container?.Register<Application>(this);
            container?.Register<Html.IImageGetter, ImageGetter>(IocLifecycle.Singleton);
            container?.Register<Android.Content.ClipboardManager>((Android.Content.ClipboardManager)GetSystemService(Context.ClipboardService)!);
            container?.Register<Android.App.NotificationManager>((Android.App.NotificationManager)GetSystemService(Context.NotificationService)!);
            // helpers
            container?.Register<IAndroidApplication>(this);
            container?.Register<ILogger>(Logger!);

            // these must be kept as singletons
            //container.Register<ICrashReporter, NullCrashReporter>(IocLifecycle.Singleton);
            //container.Register<ICrashReporter, AppCenterCrashReporter>(IocLifecycle.Singleton);
            //container.Register<IAnalyticsEngine, NullAnalyticsEngine>(IocLifecycle.Singleton);
            //container.Register<IAnalyticsEngine, FirebaseAnalyticsEngine>(IocLifecycle.Singleton);
            //container.Register<IAnalyticsEngine, AppCenterAnalyticsEngine>(IocLifecycle.Singleton);
            container?.Register<ICrashReporter, CrashlyticsReporter>(IocLifecycle.Singleton);
            container?.Register<IAnalyticsEngine, MixpanelAnalyticsEngine>(IocLifecycle.Singleton);
            container?.Register<IApplicationControlFileProvider, ApplicationControlFileProvider>(IocLifecycle.Singleton);

            container?.Register<IAndroidEnvironmentInformationProvider, AndroidEnvironmentInformationProvider>(IocLifecycle.Singleton);
            container?.Register<IResourceProvider, AndroidResourceProvider>(IocLifecycle.Singleton);
            container?.Register<IFileSystemHelper, FileSystemHelper>(IocLifecycle.Singleton);
            container?.Register<INetworkHelper, NetworkHelper>(IocLifecycle.Singleton);
            container?.Register<IPreferencesProvider, AndroidDefaultSharedPreferencesProvider>(IocLifecycle.Singleton);
            container?.Register<IUserSettings, UserSettings>(IocLifecycle.Singleton);
            container?.Register<IByteConverter, ByteConverter>(IocLifecycle.Singleton);
            container?.Register<IStatusAndProgressMessageStore, StatusAndProgressMessageStore>(IocLifecycle.Singleton);
            container?.Register<IMessageStoreInserter, MessageStoreInserter>(IocLifecycle.Singleton);
            container?.Register<IDriveVolumeInfoViewFactory, DriveVolumeInfoViewFactory>(IocLifecycle.Singleton);
            container?.Register<IApplicationControlFileFactory, ApplicationControlFileFactory>(IocLifecycle.Singleton);
            container?.Register<IValueConverter, ValueConverter>(IocLifecycle.Singleton);
            container?.Register<IValueFormatter, ValueFormatter>(IocLifecycle.Singleton);
            container?.Register<IClipboardHelper, ClipboardHelper>(IocLifecycle.Singleton);
            container?.Register<IPermissionChecker, PermissionChecker>(IocLifecycle.Singleton);

            // these must be transient
            container?.Register<IDownloadServiceController, DownloadServiceController>(IocLifecycle.PerRequest);
            container?.Register<IDownloader, Downloader>(IocLifecycle.PerRequest);

            // view models
            container?.Register<ViewModelFactory, ViewModelFactory>(IocLifecycle.Singleton);
            container?.Register<MainViewModel, MainViewModel>();
            container?.Register<SettingsViewModel, SettingsViewModel>();
            container?.Register<OpenSourceLicensesViewModel, OpenSourceLicensesViewModel>();
            container?.Register<HelpViewModel, HelpViewModel>();
            container?.Register<DownloadViewModel, DownloadViewModel>();
            container?.Register<MessagesViewModel, MessagesViewModel>();
            container?.Register<PurgeViewModel, PurgeViewModel>();
            container?.Register<EditConfigViewModel, EditConfigViewModel>();
            container?.Register<FeedDefaultsViewModel, FeedDefaultsViewModel>();
            container?.Register<GlobalValuesViewModel, GlobalValuesViewModel>();
            container?.Register<EditFeedViewModel, EditFeedViewModel>();
            container?.Register<AddFeedViewModel, AddFeedViewModel>();

            var factory = container?.Resolve<ViewModelFactory>();
            factory?.AddMap(typeof(MainViewModel));
            factory?.AddMap(typeof(SettingsViewModel));
            factory?.AddMap(typeof(OpenSourceLicensesViewModel));
            factory?.AddMap(typeof(HelpViewModel));
            factory?.AddMap(typeof(DownloadViewModel));
            factory?.AddMap(typeof(MessagesViewModel));
            factory?.AddMap(typeof(PurgeViewModel));
            factory?.AddMap(typeof(EditConfigViewModel));
            factory?.AddMap(typeof(FeedDefaultsViewModel));
            factory?.AddMap(typeof(GlobalValuesViewModel));
            factory?.AddMap(typeof(EditFeedViewModel));
            factory?.AddMap(typeof(AddFeedViewModel));
            return container;
        }

        private void SetVersionProperties()
        {
#pragma warning disable CS0618 // Type or member is obsolete
            PackageInfo? package = (
                Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu ?
                PackageManager?.GetPackageInfo(PackageName ?? "", PackageManager.PackageInfoFlags.Of((long) 0)) :
                PackageManager?.GetPackageInfo(PackageName ?? "", 0)
            );
#pragma warning restore CS0618 // Type or member is obsolete
            long longVersionCode = package != null ? PackageInfoCompat.GetLongVersionCode(package) : 0;
#if DEBUG
            var config = "(Debug)";
#else
            var config = "(Release)";
#endif
            DisplayVersion = $"v{package?.VersionName}, ({longVersionCode}), {config}";
            DisplayPackage = $"{PackageName}";
        }

        public override void OnCreate()
        {
            Log.Debug(LOGCAT_TAG, $"AndroidApplication:OnCreate SDK == {Android.OS.Build.VERSION.SdkInt}, {(int)Android.OS.Build.VERSION.SdkInt}");
            Log.Debug(LOGCAT_TAG, $"AndroidApplication:OnCreate PackageName == {this.PackageName}");

            SetupExceptionHandler();
            SetVersionProperties();
            Log.Debug(LOGCAT_TAG, $"AndroidApplication:OnCreate Version == {DisplayVersion}");

            base.OnCreate();
            var dirs = Context.GetExternalFilesDirs(null);
            if (dirs != null && dirs[0] != null)
            {
                // use our external folder - dependes on package name
                Log.Debug(LOGCAT_TAG, $"AndroidApplication:OnCreate Logs == {dirs[0].AbsolutePath}");
                LoggerFactory = new NLoggerLoggerFactory(Assets!, dirs[0].AbsolutePath);
            }
            else
            {
                // hard code and hope for the best
                LoggerFactory = new NLoggerLoggerFactory(Assets!, $"/sdcard/Android/data/{this.PackageName}/files/");
            }
            Logger = LoggerFactory.Logger;
            Logger.Debug(() => $"AndroidApplication:Logging init");

            // initialise the IoC container
            IocContainer = InitializeIocContainer();
            AddExtrasToIocContainer(IocContainer);

            Logger.Debug(() => $"AndroidApplication:IoC Init, {DisplayVersion}, {Android.OS.Build.VERSION.SdkInt}, {(int)Android.OS.Build.VERSION.SdkInt}, {this.PackageName}");
            // display the core components version
            List<string> envirnment = WindowsEnvironmentInformationProvider.GetEnvironmentRuntimeDisplayInformation();
            foreach (string line in envirnment)
            {
                Logger.Debug(() => $"AndroidApplication:{line}");
            }

            Analytics = IocContainer?.Resolve<IAnalyticsEngine>();
            Analytics?.LifecycleLaunchEvent();
        }

        public void SetLoggingNone()
        {
            // we always log errors
            LoggerFactory?.SetLoggingLevel(NLog.LogLevel.Error);
        }

        public void SetLoggingVerbose()
        {
            LoggerFactory?.SetLoggingLevel(NLog.LogLevel.Trace);
        }
    }
}