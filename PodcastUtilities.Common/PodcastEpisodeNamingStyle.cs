namespace PodcastUtilities.Common
{
    /// <summary>
    /// how to name the files as they are downloaded
    /// </summary>
    public enum PodcastEpisodeNamingStyle
    {
        /// <summary>
        /// use the filename part of the url as the name
        /// </summary>
        UrlFilename,
        /// <summary>
        /// use the filename from the url and the date time the episode was published
        /// </summary>
        UrlFilenameAndPublishDateTime,
        /// <summary>
        /// use the filename from the url, the title of the feed and the published date time
        /// </summary>
        UrlFilenameFeedTitleAndPublishDateTime
    }
}