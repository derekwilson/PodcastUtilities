using Android.App;
using Android.Runtime;
using Android.Util;
using PodcastUtilities.Common;
using PodcastUtilities.Ioc;

namespace PodcastUtilitiesPOC
{
    [Application]
    public class AndroidApplication : Application
    {
        public const string APP_NAME = "PodcastUtilities-Tag";

        protected AndroidApplication(System.IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public IIocContainer IocContainer { get; private set; }

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

        public override void OnCreate()
        {
            Log.Debug(APP_NAME, "AndroidApplication:OnCreate");
            base.OnCreate();

            // initialise the IoC container
            IocContainer = InitializeIocContainer();
            Log.Debug(APP_NAME, "AndroidApplication:IoC Init");
        }
    }
}