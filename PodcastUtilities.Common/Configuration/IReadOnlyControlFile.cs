using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using PodcastUtilities.Common.Playlists;

namespace PodcastUtilities.Common.Configuration
{
    /// <summary>
    /// this object represents the xml control file
    /// </summary>
    public interface IReadOnlyControlFile : IControlFileGlobalDefaults
    {
        /// <summary>
        /// pathname to the root folder to copy from when synchronising
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        string GetSourceRoot();

        /// <summary>
        /// pathname to the destination root folder
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        string GetDestinationRoot();

        /// <summary>
        /// filename and extension for the generated playlist
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        string GetPlaylistFileName();

        /// <summary>
        /// the format for the generated playlist
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        PlaylistFormat GetPlaylistFormat();

        /// <summary>
        /// free space in MB to leave on the destination device - when syncing
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        long GetFreeSpaceToLeaveOnDestination();

        /// <summary>
        /// free space in MB to leave on the download device - when downloading
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        long GetFreeSpaceToLeaveOnDownload();

        /// <summary>
        /// the configuration for the individual podcasts
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        IEnumerable<PodcastInfo> GetPodcasts();

        /// <summary>
        /// maximum number of background downloads
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        int GetMaximumNumberOfConcurrentDownloads();

        /// <summary>
        /// number of seconds to wait when trying a file conflict
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        int GetRetryWaitInSeconds();
    }
}
