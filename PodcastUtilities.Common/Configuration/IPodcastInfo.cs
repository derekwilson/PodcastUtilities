using System;
using System.Xml.Serialization;

namespace PodcastUtilities.Common.Configuration
{
    /// <summary>
    /// an individual podcast
    /// </summary>
    public interface IPodcastInfo : ICloneable, IXmlSerializable
    {
        /// <summary>
        /// the folder relative to the source root that contains the media for the podcast
        /// </summary>
        string Folder { get; set; }

        /// <summary>
        /// file pattern for the media files eg. *.mp3
        /// </summary>
        IDefaultableItem<string> Pattern { get; set; }

        /// <summary>
        /// field to sort on "creationtime" to use the file created time anything else to use the file name
        /// </summary>
        IDefaultableItem<PodcastFileSortField> SortField { get; set; }

        /// <summary>
        /// true for an ascending sort, false for a descending
        /// </summary>
        IDefaultableItem<bool> AscendingSort { get; set; }

        /// <summary>
        /// maximum number of files to copy, -1 for unlimited
        /// </summary>
        IDefaultableItem<int> MaximumNumberOfFiles { get; set; }

        /// <summary>
        /// the configuration info for the feed
        /// </summary>
        IFeedInfo Feed { get; set; }

        /// <summary>
        /// create a feed in the podcast
        /// </summary>
        void CreateFeed();

        /// <summary>
        /// remove a feed from the podcast
        /// </summary>
        void RemoveFeed();

        /// <summary>
        /// create a post download command in the podcast
        /// </summary>
        void CreatePostDownloadCommand();

        /// <summary>
        /// remove a post download command from the podcast
        /// </summary>
        void RemovePostDownloadCommand();
    }
}