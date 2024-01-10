using PodcastUtilities.Common.Configuration;
using PodcastUtilities.Common.Feeds;

namespace PodcastUtilities.AndroidLogic.ViewModel.Download
{
    public class DownloadRecyclerItem
    {
        public ISyncItem SyncItem { get; set; }
        public int ProgressPercentage { get; set; }
        public IPodcastInfo Podcast { get; set; }
        public bool Selected { get; set; }
        public bool AllowSelection { get; set; }
        public Status DownloadStatus { get; set; }
    }

    public enum Status
    {
        OK,
        Complete,
        Error,
        Warning,
        Information
    }

}