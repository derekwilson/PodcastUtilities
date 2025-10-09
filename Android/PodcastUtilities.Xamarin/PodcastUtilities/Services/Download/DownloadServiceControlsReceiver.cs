using Android.Content;

namespace PodcastUtilities.Services.Download
{
    [BroadcastReceiver(Enabled = true)]
    public class DownloadServiceControlsReceiver : BroadcastReceiver
    {
        public const string ACTION_STOP = "ACTION_STOP";

        public override void OnReceive(Context context, Intent intent)
        {
            if (intent != null && intent.Action != null)
            {
                switch (intent.Action)
                {
                    case ACTION_STOP:
                        context.ApplicationContext.StopService(new Intent(context.ApplicationContext, typeof(DownloadService)));
                        break;
                }
            }
        }
    }
}