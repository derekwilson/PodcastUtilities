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
        public const string LOGCAT_TAG = "PodcastUtilities-Tag";

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
            Log.Debug(LOGCAT_TAG, $"AndroidApplication:OnCreate SDK == {Android.OS.Build.VERSION.SdkInt}, {(int) Android.OS.Build.VERSION.SdkInt}");
            base.OnCreate();

            // initialise the IoC container
            IocContainer = InitializeIocContainer();
            Log.Debug(LOGCAT_TAG, "AndroidApplication:IoC Init");
        }
    }
}