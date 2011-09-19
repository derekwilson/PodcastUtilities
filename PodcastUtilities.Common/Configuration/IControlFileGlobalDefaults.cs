namespace PodcastUtilities.Common.Configuration
{
    /// <summary>
    /// the properties of the global section that are used to fill in missing values in podcasts and feeds
    /// </summary>
    public interface IControlFileGlobalDefaults
    {
        /// <summary>
        /// the global default for feeds
        /// </summary>
        int DefaultDeleteDownloadsDaysOld { get; }
    }
}