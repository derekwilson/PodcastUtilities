using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using PodcastUtilities.Common.Playlists;

namespace PodcastUtilities.Common.Configuration
{
    /// <summary>
    /// controlfile implementation that supports loading and saving
    /// </summary>
    public class PersistentControlFile : BaseControlFile, IPersistentControlFile
    {
        /// <summary>
        /// pathname to the root folder to copy from when synchronising
        /// </summary>
        public string SourceRoot
        {
            get { return SourceRootBackingField; }
            set { SourceRootBackingField = value; }
        }

        /// <summary>
        /// pathname to the destination root folder
        /// </summary>
        public string DestinationRoot
        {
            get { return DestinationRootBackingField; }
            set { DestinationRootBackingField = value; }
        }

        /// <summary>
        /// filename and extension for the generated playlist
        /// </summary>
        public string PlaylistFileName
        {
            get { return PlaylistFileNameBackingField; }
            set { PlaylistFileNameBackingField = value; }
        }

        /// <summary>
        /// the format for the generated playlist
        /// </summary>
        public PlaylistFormat PlaylistFormat
        {
            get { return PlaylistFormatBackingField; }
            set { PlaylistFormatBackingField = value; }
        }

        /// <summary>
        /// free space in MB to leave on the destination device when syncing
        /// </summary>
        public long FreeSpaceToLeaveOnDestination
        {
            get { return FreeSpaceToLeaveOnDestinationBackingField; }
            set { FreeSpaceToLeaveOnDestinationBackingField = value; }
        }

        /// <summary>
        /// free space in MB to leave on the download device - when downloading
        /// </summary>
        public long FreeSpaceToLeaveOnDownload
        {
            get { return FreeSpaceToLeaveOnDownloadBackingField; }
            set { FreeSpaceToLeaveOnDownloadBackingField = value; }
        }

        /// <summary>
        /// the configuration for the individual podcasts
        /// </summary>
        public IList<PodcastInfo> Podcasts
        {
            get { return PodcastsBackingField; }
        }

        /// <summary>
        /// maximum number of background downloads
        /// </summary>
        public int MaximumNumberOfConcurrentDownloads
        {
            get { return MaximumNumberOfConcurrentDownloadsBackingField; }
            set { MaximumNumberOfConcurrentDownloadsBackingField = value; }
        }

        /// <summary>
        /// number of seconds to wait when trying a file conflict
        /// </summary>
        public int RetryWaitInSeconds
        {
            get { return RetryWaitInSecondsBackingField; }
            set { RetryWaitInSecondsBackingField = value; }
        }

        /// <summary>
        /// persist the control file to disk
        /// </summary>
        /// <param name="fileName"></param>
        public void SaveToFile(string fileName)
        {
            throw new NotImplementedException();
        }
    }
}