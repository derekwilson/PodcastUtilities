namespace PodcastUtilities.Common
{
    /// <summary>
    /// an object that can download podcast episodes on a background thread
    /// </summary>
    public interface IPodcastEpisodeDownloader : ITask
    {
        /// <summary>
        /// the item to download
        /// </summary>
        IFeedSyncItem SyncItem { get; set; }
    }
}