using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Util;
using PodcastUtilities.Common;
using PodcastUtilities.Common.Configuration;
using PodcastUtilities.Ioc;
using PodcastUtilitiesPOC.Logging;
using PodcastUtilitiesPOC.UI;
using PodcastUtilitiesPOC.UI.Download;
using PodcastUtilitiesPOC.Utilities;
using System;
using System.Threading.Tasks;

namespace PodcastUtilitiesPOC
{
    [Application]
    public class AndroidApplication : Application
    {
        public const string LOGCAT_TAG = "PodcastUtilities-Tag";

        protected AndroidApplication(System.IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public IIocContainer IocContainer { get; private set; }
        public ILoggerFactory LoggerFactory { get; private set; }
        public ILogger Logger { get; private set; }
        public String DisplayVersion { get; private set; }
        public ReadOnlyControlFile ControlFile { get; set; }

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

        private IIocContainer AddExtrasToIocContainer(IIocContainer container)
        {
            // the container for factories
            container.Register<IIocContainer>(container);
            // android things
            container.Register<Context>(ApplicationContext);
            container.Register<Application>(this);
            // helpers
            container.Register<IPreferencesProvider, AndroidApplicationSharedPreferencesProvider>(IocLifecycle.Singleton);
            container.Register<ILogger>(Logger);
            // view models
            container.Register<ViewModelFactory, ViewModelFactory>(IocLifecycle.Singleton);
            container.Register<DownloadViewModel, DownloadViewModel>();

            var factory = container.Resolve<ViewModelFactory>();
            factory.AddMap(typeof(DownloadViewModel));
            return container;
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
            DisplayVersion = $"v{package.VersionName}, ({package.VersionCode})";
            Log.Debug(LOGCAT_TAG, $"AndroidApplication:OnCreate Version == {DisplayVersion}");

            base.OnCreate();
            LoggerFactory = new NLoggerLoggerFactory();
            Logger = LoggerFactory.Logger;
            Logger.Debug(() => $"AndroidApplication:Logging init");

            // initialise the IoC container
            IocContainer = InitializeIocContainer();
            AddExtrasToIocContainer(IocContainer);
            Logger.Debug(() => $"AndroidApplication:IoC Init, {DisplayVersion}, {Android.OS.Build.VERSION.SdkInt}, {(int)Android.OS.Build.VERSION.SdkInt}, {this.PackageName}");
        }
    }
}