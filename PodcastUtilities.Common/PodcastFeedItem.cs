using System;

namespace PodcastUtilities.Common
{
    /// <summary>
    /// An item in a podcast feed
    /// </summary>
    class PodcastFeedItem : IPodcastFeedItem
    {
        /// <summary>
        /// title of the item
        /// </summary>
        public string Title  { get; set; }

        /// <summary>
        /// address of the attachment to download
        /// </summary>
        public Uri Address { get; set; }

        /// <summary>
        /// filename to use when saving the podcast file
        /// </summary>
        /// <returns></returns>
        public string GetFilename()
        {
            throw new NotImplementedException();
        }
    }
}