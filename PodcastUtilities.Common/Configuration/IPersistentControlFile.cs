using System.Collections.Generic;
using PodcastUtilities.Common.Playlists;

namespace PodcastUtilities.Common.Configuration
{
    /// <summary>
    /// this object represents the xml control file
    /// </summary>
    public interface IPersistentControlFile
    {
        /// <summary>
        /// pathname to the root folder to copy from when synchronising
        /// </summary>
        string SourceRoot { get; set; }

        /// <summary>
        /// pathname to the destination root folder
        /// </summary>
        string DestinationRoot { get; set; }

        /// <summary>
        /// filename and extension for the generated playlist
        /// </summary>
        string PlaylistFileName { get; set; }

        /// <summary>
        /// the format for the generated playlist
        /// </summary>
        PlaylistFormat PlaylistFormat { get; set; }

        /// <summary>
        /// free space in MB to leave on the destination device - when syncing
        /// </summary>
        long FreeSpaceToLeaveOnDestination { get; set; }

        /// <summary>
        /// free space in MB to leave on the download device - when downloading
        /// </summary>
        long FreeSpaceToLeaveOnDownload { get; set; }

        /// <summary>
        /// the configuration for the individual podcasts
        /// </summary>
        IList<PodcastInfo> Podcasts { get; }

        /// <summary>
        /// maximum number of background downloads
        /// </summary>
        int MaximumNumberOfConcurrentDownloads { get; set; }

        /// <summary>
        /// number of seconds to wait when trying a file conflict
        /// </summary>
        int RetryWaitInSeconds { get; set; }

        /// <summary>
        /// persist the control file to disk
        /// </summary>
        /// <param name="fileName"></param>
        void SaveToFile(string fileName);
    }
}