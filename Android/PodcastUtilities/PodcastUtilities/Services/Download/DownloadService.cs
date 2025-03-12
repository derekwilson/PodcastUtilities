using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using PodcastUtilities.AndroidLogic.Services.Download;

namespace PodcastUtilities.Services.Download
{
    [Service]
    public class DownloadService : Service, IDownloadService
	{
        private AndroidApplication AndroidApplication;
        private DownloadServiceBinder binder;

        public override void OnCreate()
        {
            IsDownloading = false;
            AndroidApplication = Application as AndroidApplication;
            AndroidApplication.Logger.Debug(() => "DownloadService:OnCreate");
            base.OnCreate();
        }

        public override IBinder OnBind(Intent intent)
        {
            binder = new DownloadServiceBinder(this);
            AndroidApplication.Logger.Debug(() => "DownloadService:OnBind");
            return binder;
        }

        public override bool OnUnbind(Intent intent)
        {
            AndroidApplication.Logger.Debug(() => "DownloadService:OnUnbind");
            return base.OnUnbind(intent);
        }

        public override void OnDestroy()
        {
            AndroidApplication.Logger.Debug(() => "DownloadService:OnDestroy");
            base.OnDestroy();
        }

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            AndroidApplication.Logger.Debug(() => "DownloadService:OnStartCommand");
            return StartCommandResult.Sticky;
            //return base.OnStartCommand(intent, flags, startId);
        }

        public bool IsDownloading { get; private set; }
    }
}