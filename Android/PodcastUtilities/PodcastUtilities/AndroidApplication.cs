using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Util;
using PodcastUtilities.AndroidLogic.Logging;
using PodcastUtilities.AndroidLogic.Utilities;
using PodcastUtilities.AndroidLogic.ViewModel;
using PodcastUtilities.AndroidLogic.ViewModel.Main;
using PodcastUtilities.Common;
using PodcastUtilities.Common.Platform;
using PodcastUtilities.Ioc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PodcastUtilities
{
    [Application]
    public class AndroidApplication : Application
    {
        // tag used in LogCat, only used until NLog starts running
        public const string LOGCAT_TAG = "PodcastUtilities-Tag";

        public ILoggerFactory LoggerFactory { get; private set; }
        public ILogger Logger { get; private set; }
        public String DisplayVersion { get; private set; }
        public IIocContainer IocContainer { get; private set; }

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
        private IIocContainer AddExtrasToIocContainer(IIocContainer container)
        {
            // the container itself, for factories
            container.Register<IIocContainer>(container);
            // android things
            container.Register<Context>(ApplicationContext);
            container.Register<Application>(this);
            // helpers
            container.Register<ILogger>(Logger);
            container.Register<IResourceProvider, AndroidResourceProvider>(IocLifecycle.Singleton);
            // view models
            container.Register<ViewModelFactory, ViewModelFactory>(IocLifecycle.Singleton);
            container.Register<MainViewModel, MainViewModel>();

            var factory = container.Resolve<ViewModelFactory>();
            factory.AddMap(typeof(MainViewModel));
            return container;
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
            var dirs = Context.GetExternalFilesDirs(null);
            if (dirs != null && dirs[0] != null)
            {
                // use our external folder - dependes on package name
                Log.Debug(LOGCAT_TAG, $"AndroidApplication:OnCreate Logs == {dirs[0].AbsolutePath}");
                LoggerFactory = new NLoggerLoggerFactory(dirs[0].AbsolutePath);
            }
            else
            {
                // hard code and hope for the best
                LoggerFactory = new NLoggerLoggerFactory($"/sdcard/Android/data/{this.PackageName}/files/");
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

        }
    }
}