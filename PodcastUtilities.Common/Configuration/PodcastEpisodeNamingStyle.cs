namespace PodcastUtilities.Common.Configuration
{
    /// <summary>
    /// how to name the files as they are downloaded
    /// </summary>
    public enum PodcastEpisodeNamingStyle
    {
        /// <summary>
        /// use the filename part of the url as the name
        /// </summary>
        UrlFileName,
        /// <summary>
        /// use the filename from the url and the date time the episode was published
        /// </summary>
        UrlFileNameAndPublishDateTime,
        /// <summary>
        /// use the filename from the url, the title of the feed and the published date time
        /// </summary>
        UrlFileNameFeedTitleAndPublishDateTime,
        /// <summary>
        /// as UrlFilenameFeedTitleAndPublishDateTime but each month is put in a folder
        /// </summary>
        UrlFileNameFeedTitleAndPublishDateTimeInfolder,
        /// <summary>
        /// use the podcast title as the name
        /// </summary>
        EpisodeTitle,
        /// <summary>
        /// use the published date time and the episode title
        /// </summary>
        EpisodeTitleAndPublishDateTime,
    }
}