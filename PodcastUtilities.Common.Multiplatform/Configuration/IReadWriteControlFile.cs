#region License
// FreeBSD License
// Copyright (c) 2010 - 2013, Andrew Trevarrow and Derek Wilson
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
// 
// Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
// 
// Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED 
// WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A 
// PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR
// ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED
// TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING 
// NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
// POSSIBILITY OF SUCH DAMAGE.
#endregion
using System;
using System.Xml.Serialization;
using PodcastUtilities.Common.Playlists;

namespace PodcastUtilities.Common.Configuration
{
    /// <summary>
    /// this object represents the xml control file
    /// </summary>
    public interface IReadWriteControlFile : IReadOnlyControlFile, ICloneable, IXmlSerializable
    {
        /// <summary>
        /// the global default for feeds
        /// </summary>
        void SetDefaultDeleteDownloadsDaysOld(int deleteDaysOld);

        /// <summary>
        /// the global default for feeds
        /// </summary>
        void SetDefaultMaximumNumberOfDownloadedItems(int maxItems);

        /// <summary>
        /// the global default for feeds
        /// </summary>
        void SetDefaultDownloadStrategy(PodcastEpisodeDownloadStrategy strategy);

        /// <summary>
        /// the global default for feeds
        /// </summary>
        void SetDefaultFeedFormat(PodcastFeedFormat format);

        /// <summary>
        /// the global default for feeds
        /// </summary>
        void SetDefaultMaximumDaysOld(int maximumDaysOld);

        /// <summary>
        /// the global default for feeds
        /// </summary>
        void SetDefaultNamingStyle(PodcastEpisodeNamingStyle namingStyle);

        /// <summary>
        /// the global default for podcasts
        /// </summary>
        void SetDefaultNumberOfFiles(int numberOfFiles);

        /// <summary>
        /// the global default for podcasts
        /// </summary>
        void SetDefaultFilePattern(string pattern);

        /// <summary>
        /// the global default for podcasts
        /// </summary>
        void SetDefaultDeleteEmptyFolder(bool delete);

        /// <summary>
        /// the global default for podcasts
        /// </summary>
        void SetDefaultAscendingSort(bool ascendingSort);

        /// <summary>
        /// the global default for podcasts
        /// </summary>
        void SetDefaultSortField(PodcastFileSortField sortField);

        /// <summary>
        /// the global default for post download command
        /// </summary>
        void SetDefaultPostDownloadCommand(string command);

        /// <summary>
        /// the global default for post download command args
        /// </summary>
        void SetDefaultPostDownloadArguments(string command);

        /// <summary>
        /// the global default for post download command cwd
        /// </summary>
        void SetDefaultPostDownloadWorkingDirectory(string command);

        /// <summary>
        /// level of diagnostic output
        /// </summary>
        void SetDiagnosticOutput(DiagnosticOutputLevel level);

        /// <summary>
        /// set to retain intermediate files
        /// </summary>
        void SetDiagnosticRetainTemporaryFiles(bool retainFiles);

        /// <summary>
        /// pathname to the root folder to copy from when synchronising
        /// </summary>
        void SetSourceRoot(string value);

        /// <summary>
        /// pathname to the destination root folder
        /// </summary>
        void SetDestinationRoot(string value);

        /// <summary>
        /// filename and extension for the generated playlist
        /// </summary>
        void SetPlaylistFileName(string value);

        /// <summary>
        /// the format for the generated playlist
        /// </summary>
        void SetPlaylistFormat(PlaylistFormat value);

		/// <summary>
		/// path separator to use in the generated playlists
		/// </summary>
		void SetPlaylistPathSeparator(string value);

		/// <summary>
		/// free space in MB to leave on the destination device when syncing
		/// </summary>
		void SetFreeSpaceToLeaveOnDestination(long value);

        /// <summary>
        /// free space in MB to leave on the download device - when downloading
        /// </summary>
        void SetFreeSpaceToLeaveOnDownload(long value);

        /// <summary>
        /// maximum number of background downloads
        /// </summary>
        void SetMaximumNumberOfConcurrentDownloads(int value);

        /// <summary>
        /// number of seconds to wait when trying a file conflict
        /// </summary>
        void SetRetryWaitInSeconds(int value);

        /// <summary>
        /// persist the control file to disk
        /// </summary>
        /// <param name="fileName"></param>
        void SaveToFile(string fileName);

        /// <summary>
        /// add a new podcast to the configuration
        /// </summary>
        /// <param name="podcast"></param>
        void AddPodcast(IPodcastInfo podcast);

        /// <summary>
        /// remove a podcast from the configuration
        /// </summary>
        /// <param name="podcast"></param>
        void DeletePodcast(IPodcastInfo podcast);
    }
}