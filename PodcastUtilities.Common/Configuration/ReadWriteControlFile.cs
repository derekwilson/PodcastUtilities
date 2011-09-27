using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using PodcastUtilities.Common.Playlists;

namespace PodcastUtilities.Common.Configuration
{
    /// <summary>
    /// controlfile implementation that supports loading and saving
    /// </summary>
    public class ReadWriteControlFile : BaseControlFile, IReadWriteControlFile
    {
        /// <summary>
        /// pathname to the root folder to copy from when synchronising
        /// </summary>
        public void SetSourceRoot(string value)
        {
            SourceRoot = value;
        }

        /// <summary>
        /// pathname to the destination root folder
        /// </summary>
        public void SetDestinationRoot(string value)
        {
            DestinationRoot = value;
        }

        /// <summary>
        /// filename and extension for the generated playlist
        /// </summary>
        public void SetPlaylistFileName(string value)
        {
            PlaylistFileName = value;
        }

        /// <summary>
        /// the format for the generated playlist
        /// </summary>
        public void SetPlaylistFormat(PlaylistFormat value)
        {
            PlaylistFormat = value;
        }

        /// <summary>
        /// free space in MB to leave on the destination device when syncing
        /// </summary>
        public void SetFreeSpaceToLeaveOnDestination(long value)
        {
            FreeSpaceToLeaveOnDestination = value;
        }

        /// <summary>
        /// free space in MB to leave on the download device - when downloading
        /// </summary>
        public void SetFreeSpaceToLeaveOnDownload(long value)
        {
            FreeSpaceToLeaveOnDownload = value;
        }

        /// <summary>
        /// maximum number of background downloads
        /// </summary>
        public void SetMaximumNumberOfConcurrentDownloads(int value)
        {
            MaximumNumberOfConcurrentDownloads = value;
        }

        /// <summary>
        /// number of seconds to wait when trying a file conflict
        /// </summary>
        public void SetRetryWaitInSeconds(int value)
        {
            RetryWaitInSeconds = value;
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