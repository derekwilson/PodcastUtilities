namespace PodcastUtilities.Common.Feeds
{
    /// <summary>
    /// an object that can download podcast episodes on a background thread
    /// </summary>
    public interface IEpisodeDownloader : ITask
    {
        /// <summary>
        /// the item to download
        /// </summary>
        ISyncItem SyncItem { get; set; }
    }
}