using Android.OS;

namespace PodcastUtilities.Services.Download
{
	public class DownloadServiceBinder : Binder
	{
        public DownloadService service;

        public DownloadServiceBinder(DownloadService svc)
        {
            this.service = svc;
        }
    }
}