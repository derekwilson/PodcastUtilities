using System.Collections.Generic;
using PodcastUtilities.AndroidLogic.ViewModel.Download;

namespace PodcastUtilities.AndroidLogic.Services.Download
{
	public interface IDownloadService
	{
        bool IsDownloading { get; }

        void StartDownloads(List<DownloadRecyclerItem> allItems);

        void CancelDownloads();

        void KillNotification();

        List<DownloadRecyclerItem> GetItems();

        DownloaderEvents GetDownloaderEvents();
    }
}