using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Xml;
using System.Xml.XPath;
using PodcastUtilities.Common.Exceptions;
using PodcastUtilities.Common.Playlists;

namespace PodcastUtilities.Common.Configuration
{
    /// <summary>
	/// this object represents the xml control file
	/// </summary>
    public class ReadOnlyControlFile : BaseControlFile, IReadOnlyControlFile
	{
		/// <summary>
		/// create the object and read the control file from the specified filename
		/// </summary>
		/// <param name="fileName">pathname to the control file xml</param>
        public ReadOnlyControlFile(string fileName)
		{
            LoadFromFile(fileName);
		}

        /// <summary>
        /// only used for unit testing
        /// </summary>
        public ReadOnlyControlFile(IXPathNavigable document)
		{
            LoadFromXml(document);
		}

		/// <summary>
		/// pathname to the root folder to copy from when synchronising
		/// </summary>
        public string SourceRoot
		{
            get { return SourceRootBackingField; }
		}
		
		/// <summary>
		/// pathname to the destination root folder
		/// </summary>
        public string DestinationRoot
		{
            get { return DestinationRootBackingField; }
		}

		/// <summary>
		/// filename and extension for the generated playlist
		/// </summary>
        public string PlaylistFileName
		{
            get { return PlaylistFileNameBackingField; }
		}

        /// <summary>
        /// the format for the generated playlist
        /// </summary>
        public PlaylistFormat PlaylistFormat
        {
            get { return PlaylistFormatBackingField; }
        }

        /// <summary>
        /// free space in MB to leave on the destination device when syncing
        /// </summary>
        public long FreeSpaceToLeaveOnDestination
        {
            get { return FreeSpaceToLeaveOnDestinationBackingField; }
        }

        /// <summary>
        /// free space in MB to leave on the download device - when downloading
        /// </summary>
        public long FreeSpaceToLeaveOnDownload
        {
            get { return FreeSpaceToLeaveOnDownloadBackingField; }
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
        }

        /// <summary>
        /// number of seconds to wait when trying a file conflict
        /// </summary>
        public int RetryWaitInSeconds
        {
            get { return RetryWaitInSecondsBackingField; }
        }
	}
}
