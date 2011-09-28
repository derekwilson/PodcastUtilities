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
        string Pattern { get; set; }

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
        int MaximumNumberOfFiles { get; set; }

        /// <summary>
        /// the configuration info for the feed
        /// </summary>
        IFeedInfo Feed { get; set; }

        /// <summary>
        /// create a feed in the podcast
        /// </summary>
        void CreateFeed();
    }
}