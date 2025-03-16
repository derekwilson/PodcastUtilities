using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using AndroidX.Core.App;
using PodcastUtilities.AndroidLogic.Services.Download;
using PodcastUtilities.AndroidLogic.ViewModel.Download;
using PodcastUtilities.Common.Feeds;
using PodcastUtilities.UI.Download;
using System;
using System.Collections.Generic;

namespace PodcastUtilities.Services.Download
{
    [Service]
    public class DownloadService : Service, IDownloadService
	{
        private const int FOREGROUND_NOTIFICATION_ID = 100;
        private const string NOTIFICATION_CHANNEL_ID = "podcastutilities-download-service-channel-id-01";

        private AndroidApplication AndroidApplication;
        private DownloadServiceBinder Binder;

        // injected
        private NotificationManager NotificationManager;
        private AndroidLogic.Services.Download.IDownloader Downloader;
        private DownloaderEvents DownloaderEvents;

        public override void OnCreate()
        {
            AndroidApplication = Application as AndroidApplication;
            AndroidApplication.Logger.Debug(() => "DownloadService:OnCreate");

            base.OnCreate();

            // maybe there is a better way to do the injection
            Downloader = AndroidApplication.IocContainer.Resolve<AndroidLogic.Services.Download.IDownloader>();
            DownloaderEvents = Downloader.GetDownloaderEvents();
            AttachDownloaderEvents();
            NotificationManager = AndroidApplication.IocContainer.Resolve<NotificationManager>();
            AndroidApplication.Logger.Debug(() => "DownloadService:OnCreate - Complete");
        }

        private void AttachDownloaderEvents()
        {
            if (DownloaderEvents == null)
            {
                return;
            }
            DownloaderEvents.UpdateItemProgressEvent += DownloaderProgress;
            DownloaderEvents.UpdateItemStatusEvent += DownloaderStatus;
            DownloaderEvents.CompleteEvent += DownloaderComplete;
        }

        private void DetachDownloaderEvents()
        {
            if (DownloaderEvents == null)
            {
                return;
            }
            DownloaderEvents.CompleteEvent -= DownloaderComplete;
            DownloaderEvents.UpdateItemProgressEvent -= DownloaderProgress;
            DownloaderEvents.UpdateItemStatusEvent -= DownloaderStatus;
        }

        private void DownloaderComplete(object sender, EventArgs e)
        {
            AndroidApplication.Logger.Debug(() => "DownloadService:DownloaderComplete");
            UpdateNotification();
            StopSelf();
            AndroidApplication.Logger.Debug(() => "DownloadService:DownloaderComplete - complete");
        }

        private void DownloaderStatus(object sender, Tuple<Common.Feeds.ISyncItem, Status, string> e)
        {
            (ISyncItem item, Status status, string message) = e;
            if (status == Status.Complete || status == Status.Error)
            {
                AndroidApplication.Logger.Debug(() => "DownloadService:DownloaderStatus");
                UpdateNotification();
            }
        }

        private void DownloaderProgress(object sender, Tuple<Common.Feeds.ISyncItem, int> e)
        {
            (ISyncItem item, int progressPercentage) = e;
            if (progressPercentage == 100)
            {
                AndroidApplication.Logger.Debug(() => "DownloadService:DownloaderProgress");
                UpdateNotification();
            }
        }

        public override IBinder OnBind(Intent intent)
        {
            Binder = new DownloadServiceBinder(this);
            AndroidApplication.Logger.Debug(() => "DownloadService:OnBind");
            return Binder;
        }

        public override bool OnUnbind(Intent intent)
        {
            AndroidApplication.Logger.Debug(() => "DownloadService:OnUnbind");
            return base.OnUnbind(intent);
        }

        public override void OnDestroy()
        {
            AndroidApplication.Logger.Debug(() => "DownloadService:OnDestroy");
            DetachDownloaderEvents();
            Downloader?.CancelAll();
            base.OnDestroy();
        }

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            AndroidApplication.Logger.Debug(() => "DownloadService:OnStartCommand");
            return StartCommandResult.Sticky;
        }

        private void StartForeground()
        {
            AndroidApplication.Logger.Debug(() => "DownloadService:StartForeground - start");
            StartForeground(FOREGROUND_NOTIFICATION_ID, GetForegroundNotification());
            AndroidApplication.Logger.Debug(() => "DownloadService:StartForeground - end");
        }

        private void UpdateNotification()
        {
            AndroidApplication.Logger.Debug(() => "DownloadService:UpdateNotification - start");
            NotificationManager.Notify(FOREGROUND_NOTIFICATION_ID, GetForegroundNotification());
            AndroidApplication.Logger.Debug(() => "DownloadService:UpdateNotification - end");
        }

        private void CreateChannel()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var notificationChannel = new NotificationChannel(NOTIFICATION_CHANNEL_ID, "Trailblazer Notifications", NotificationImportance.Default);

                // Configure the notification channel.
                notificationChannel.Description = "Track recording updates";
                notificationChannel.EnableLights(false);
                notificationChannel.EnableVibration(false);
                NotificationManager.CreateNotificationChannel(notificationChannel);
            }
        }

        private Notification GetForegroundNotification()
        {
            CreateChannel();

            var builder = new NotificationCompat.Builder(this, NOTIFICATION_CHANNEL_ID);

            builder.SetOngoing(true);
            builder.SetContentTitle(Downloader?.NotifcationTitle ?? "ERROR");
            builder.SetContentText(Downloader?.NotifcationText ?? "ERROR");
            builder.SetTicker(Downloader?.NotifcationAccessibilityText ?? "ERROR");
            builder.SetWhen(Java.Lang.JavaSystem.CurrentTimeMillis());
            builder.SetShowWhen(true);
            builder.SetSmallIcon(Resource.Drawable.ic_download);
            //builder.SetColor(ContextCompat.getColor(this, R.color.indigo));
            builder.SetVisibility(NotificationCompat.VisibilityPublic);        // so it will show on the lock screen
            builder.SetContentIntent(GetNotificationIntent());
            builder.SetSilent(true);

            builder.AddAction(
                Resource.Drawable.ic_clear,
                GetString(Resource.String.action_cancel),
                GetButtonIntent(DownloadServiceControlsReceiver.ACTION_STOP)
            );

            return builder.Build();
        }

        private PendingIntent GetNotificationIntent()
        {
            var intent = DownloadActivity.CreateIntentFromHud(this);
            intent.SetAction(Intent.ActionMain);
            intent.AddCategory(Intent.CategoryLauncher);
            intent.AddFlags(ActivityFlags.NewTask);

            return PendingIntent.GetActivity(
                this,
                0,
                intent,
                PendingIntentFlags.Immutable
            );
        }

        private PendingIntent GetButtonIntent(string action)
        {
            var intent = new Intent(this, typeof(DownloadServiceControlsReceiver));
            intent.SetAction(action);

            return PendingIntent.GetBroadcast(
                this,
                FOREGROUND_NOTIFICATION_ID,
                intent,
                PendingIntentFlags.CancelCurrent | PendingIntentFlags.Immutable);
        }

        public void StartDownloads(List<DownloadRecyclerItem> allItems)
        {
            Downloader?.SetDownloadingItems(allItems);
            StartForeground();      // we need to create the notification
            Downloader?.DownloadAllItems();
        }

        public DownloaderEvents GetDownloaderEvents()
        {
            return Downloader?.GetDownloaderEvents();
        }

        public List<DownloadRecyclerItem> GetItems()
        {
            return Downloader?.GetDownloadItems();
        }

        public void CancelDownloads()
        {
            Downloader?.CancelAll();
        }

        public bool IsDownloading
        {
            get
            {
                return Downloader?.IsDownloading ?? false;
            }
        }
    }
}