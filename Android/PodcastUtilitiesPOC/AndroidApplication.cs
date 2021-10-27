using Android.App;
using Android.Runtime;
using Android.Util;
using PodcastUtilities.Common;
using PodcastUtilities.Ioc;
using PodcastUtilitiesPOC.Logging;
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

        private static IIocContainer AddExtrasToIocContainer(IIocContainer container)
        {
            container.Register<ILoggerFactory, NLoggerLoggerFactory>(IocLifecycle.Singleton);
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
            base.OnCreate();

            // initialise the IoC container
            IocContainer = InitializeIocContainer();
            AddExtrasToIocContainer(IocContainer);
            LoggerFactory = IocContainer.Resolve<ILoggerFactory>();
            Logger = LoggerFactory.Logger;
            Logger.Debug(() => "AndroidApplication:IoC Init");
        }
    }
}