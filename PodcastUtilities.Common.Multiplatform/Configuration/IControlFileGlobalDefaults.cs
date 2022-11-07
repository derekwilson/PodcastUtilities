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
using System.Diagnostics.CodeAnalysis;
namespace PodcastUtilities.Common.Configuration
{
    /// <summary>
    /// the properties of the global section that are used to fill in missing values in podcasts and feeds
    /// </summary>
    public interface IControlFileGlobalDefaults
    {
        /// <summary>
        /// the global default for feeds
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        int GetDefaultDeleteDownloadsDaysOld();

        /// <summary>
        /// the global default for feeds
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        int GetDefaultMaximumNumberOfDownloadedItems();

        /// <summary>
        /// the global default for feeds
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        PodcastEpisodeDownloadStrategy GetDefaultDownloadStrategy();

        /// <summary>
        /// the global default for feeds
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        PodcastFeedFormat GetDefaultFeedFormat();

        /// <summary>
        /// the global default for feeds
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        int GetDefaultMaximumDaysOld();

        /// <summary>
        /// the global default for feeds
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        PodcastEpisodeNamingStyle GetDefaultNamingStyle();

        /// <summary>
        /// the global default for podcasts
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        int GetDefaultNumberOfFiles();

        /// <summary>
        /// the global default for podcasts
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        string GetDefaultFilePattern();

        /// <summary>
        /// the global default for podcasts
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        bool GetDefaultDeleteEmptyFolder();

        /// <summary>
        /// the global default for podcasts
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        bool GetDefaultAscendingSort();

        /// <summary>
        /// the global default for podcasts
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        PodcastFileSortField GetDefaultSortField();

        /// <summary>
        /// the global default for post download command
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        string GetDefaultPostDownloadCommand();

        /// <summary>
        /// the global default for post download command args
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        string GetDefaultPostDownloadArguments();

        /// <summary>
        /// the global default for post download command cwd
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        string GetDefaultPostDownloadWorkingDirectory();
    }
}