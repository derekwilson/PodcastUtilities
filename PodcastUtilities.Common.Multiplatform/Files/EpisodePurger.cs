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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using PodcastUtilities.Common.Configuration;
using PodcastUtilities.Common.Exceptions;
using PodcastUtilities.Common.Platform;

namespace PodcastUtilities.Common.Files
{
    /// <summary>
    /// purge old download files
    /// </summary>
    public class EpisodePurger : IEpisodePurger
    {
        private string[] _filesThatCanBeAutomaticallyDeleted = new string[]
        {
            "albumartsmall.jpg", 
            "folder.jpg", 
            "thumbs.db"
        };

        private readonly IDirectoryInfoProvider _directoryInfoProvider;
        private readonly ITimeProvider _timeProvider;
        private readonly IFileUtilities _fileUtilities;

        /// <summary>
        /// create the purger
        /// </summary>
        public EpisodePurger(ITimeProvider timeProvider, IDirectoryInfoProvider directoryInfoProvider, IFileUtilities fileUtilities)
        {
            _directoryInfoProvider = directoryInfoProvider;
            _fileUtilities = fileUtilities;
            _timeProvider = timeProvider;
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private static DateTime GetWhenDownloadWasPublished(IPodcastInfo podcastInfo, IFileInfo file)
        {
            switch (podcastInfo.Feed.NamingStyle.Value)
            {
                case PodcastEpisodeNamingStyle.UrlFileNameAndPublishDateTime:
                case PodcastEpisodeNamingStyle.UrlFileNameFeedTitleAndPublishDateTime:
                case PodcastEpisodeNamingStyle.EpisodeTitleAndPublishDateTime:
                case PodcastEpisodeNamingStyle.UrlFileNameFeedTitleAndPublishDateTimeInfolder:
                    try
                    {
                        return ConvertFilenameToPublishedDate(Path.GetFileNameWithoutExtension(file.FullName));
                    }
                    catch (Exception)
                    {
                        return file.CreationTime;
                    }
                case PodcastEpisodeNamingStyle.EpisodeTitle:
                case PodcastEpisodeNamingStyle.UrlFileName:
                    return file.CreationTime;
                default:
                    throw new EnumOutOfRangeException("NamingStyle");
            }
        }

        private static DateTime ConvertFilenameToPublishedDate(string fileName)
        {
            return new DateTime(
                    Convert.ToInt32(fileName.Substring(0, 4), CultureInfo.InvariantCulture),
                    Convert.ToInt32(fileName.Substring(5, 2), CultureInfo.InvariantCulture),
                    Convert.ToInt32(fileName.Substring(8, 2), CultureInfo.InvariantCulture),
                    Convert.ToInt32(fileName.Substring(11, 2), CultureInfo.InvariantCulture),
                    Convert.ToInt32(fileName.Substring(13, 2), CultureInfo.InvariantCulture),
                    0
                );
        }

        /// <summary>
        /// find old downloads that can be deleted
        /// </summary>
        /// <param name="rootFolder">the root folder for all downloads</param>
        /// <param name="podcastInfo">info on the podcast to download</param>
        /// <returns>list of episodes to be deleted for the supplied podcastInfo</returns>
        public IList<IFileInfo> FindEpisodesToPurge(string rootFolder, IPodcastInfo podcastInfo)
        {
            List<IFileInfo> episodesToDelete = new List<IFileInfo>(10);
            if (podcastInfo.Feed == null)
            {
                // it is optional to have a feed
                return episodesToDelete;
            }

            var feedDownloadsFolder = Path.Combine(rootFolder, podcastInfo.Folder);
            var oldestEpisodeToKeep = DateTime.MinValue;
            if (podcastInfo.Feed.DeleteDownloadsDaysOld.Value < int.MaxValue)
            {
                oldestEpisodeToKeep = _timeProvider.UtcNow.AddDays(-podcastInfo.Feed.DeleteDownloadsDaysOld.Value);
            }

            if (IsSubFolderBasedNaming(podcastInfo.Feed.NamingStyle.Value))
            {
                ScanSubFoldersForOldFiles(feedDownloadsFolder, oldestEpisodeToKeep, episodesToDelete, podcastInfo);
            }
            else
            {
                ScanFolderForOldFiles(feedDownloadsFolder,oldestEpisodeToKeep,episodesToDelete,podcastInfo);
            }

            return episodesToDelete;
        }

        private static bool IsSubFolderBasedNaming(PodcastEpisodeNamingStyle style)
        {
            return style == PodcastEpisodeNamingStyle.UrlFileNameFeedTitleAndPublishDateTimeInfolder;
        }

        private void ScanSubFoldersForOldFiles(string folderToScan, DateTime oldestEpisodeToKeep, List<IFileInfo> episodesToDelete, IPodcastInfo podcastInfo)
        {
            IDirectoryInfo directoryInfo = _directoryInfoProvider.GetDirectoryInfo(folderToScan);

            IDirectoryInfo[] subFolders;
            try
            {
                subFolders = directoryInfo.GetDirectories("*.*");
            }
            catch (DirectoryNotFoundException)
            {
                // if the folder is not there then there is nothing to do
                return;
            }

            foreach (IDirectoryInfo subFolder in subFolders)
            {
                ScanFolderForOldFiles(subFolder.FullName,oldestEpisodeToKeep,episodesToDelete,podcastInfo);
            }
        }

        private void ScanFolderForOldFiles(string folderToScan, DateTime oldestEpisodeToKeep, List<IFileInfo> episodesToDelete, IPodcastInfo podcastInfo)
        {
            IDirectoryInfo directoryInfo = _directoryInfoProvider.GetDirectoryInfo(folderToScan);

            IFileInfo[] files;
            try
            {
                files = directoryInfo.GetFiles(podcastInfo.Pattern.Value);
            }
            catch (DirectoryNotFoundException)
            {
                // if the folder is not there then there is nothing to do
                return;
            }

            foreach (IFileInfo file in files)
            {
                var extension = Path.GetExtension(file.FullName);
                if (extension != null && extension.ToUpperInvariant() == ".XML")
                {
                    // do not delete the state file
                    continue;
                }
                if (GetWhenDownloadWasPublished(podcastInfo, file) < oldestEpisodeToKeep)
                    episodesToDelete.Add(file);
            }
        }

        /// <summary>
        /// Find folders that will be empty and that can be purged
        /// </summary>
        /// <param name="rootFolder">the root folder for all downloads</param>
        /// <param name="podcastInfo">info on the podcast to download</param>
        /// <param name="filesThatWillBeDeleted">files that will be removed and do not count when considering an empty folder</param>
        /// <returns></returns>
        public IList<IDirectoryInfo> FindEmptyFoldersToDelete(string rootFolder, IPodcastInfo podcastInfo, IList<IFileInfo> filesThatWillBeDeleted)
        {
            List<IDirectoryInfo> foldersToDelete = new List<IDirectoryInfo>(10);
            if (!podcastInfo.DeleteEmptyFolder.Value)
            {
                return foldersToDelete;
            }

            var feedDownloadsFolder = Path.Combine(rootFolder, podcastInfo.Folder);
            IDirectoryInfo directoryInfo = _directoryInfoProvider.GetDirectoryInfo(feedDownloadsFolder);

            if (podcastInfo.Feed != null && IsSubFolderBasedNaming(podcastInfo.Feed.NamingStyle.Value))
            {
                CheckIfSubFoldersCanBeDeleted(directoryInfo, filesThatWillBeDeleted, foldersToDelete);
            }
            else
            {
                if (FolderCanBeDeleted(directoryInfo, filesThatWillBeDeleted))
                {
                    foldersToDelete.Add(directoryInfo);
                }
            }
            return foldersToDelete;
        }

        private void CheckIfSubFoldersCanBeDeleted(IDirectoryInfo rootFolder, IList<IFileInfo> filesToBeDeleted, List<IDirectoryInfo> foldersToDelete)
        {
            IDirectoryInfo[] subFolders;
            try
            {
                subFolders = rootFolder.GetDirectories("*.*");
            }
            catch (DirectoryNotFoundException)
            {
                // lets see if we can get rid of the root folder
                if (FolderCanBeDeleted(rootFolder, filesToBeDeleted))
                {
                    foldersToDelete.Add(rootFolder);
                }
                return;
            }

            bool thereIsASubFolderToKeep = false;
            foreach (IDirectoryInfo subFolder in subFolders)
            {
                if (FolderCanBeDeleted(subFolder, filesToBeDeleted))
                {
                    foldersToDelete.Add(subFolder);
                }
                else
                {
                    thereIsASubFolderToKeep = true;
                }
            }
            if (thereIsASubFolderToKeep)
            {
                // the root cannot be deleted
                return;
            }
            if (FolderCanBeDeleted(rootFolder, filesToBeDeleted))
            {
                foldersToDelete.Add(rootFolder);
            }
        }

        private bool FolderCanBeDeleted(IDirectoryInfo folderToScan, IList<IFileInfo> filesToBeDeleted)
        {
            IFileInfo[] files;
            try
            {
                files = folderToScan.GetFiles("*.*");
            }
            catch (DirectoryNotFoundException)
            {
                // if the folder is not there then there is nothing to do
                return false;
            }

            if (files.Length < 1)
            {
                return true;
            }

            foreach (IFileInfo file in files)
            {
                if (_filesThatCanBeAutomaticallyDeleted.Contains(file.Name.ToLower(CultureInfo.InvariantCulture)))
                {
                    // this file can be ignored as it is an auto generated file
                    continue;
                }
                if (filesToBeDeleted != null && filesToBeDeleted.Any(
                            alreadyGone =>
                                alreadyGone.Name.ToLower(CultureInfo.InvariantCulture) == file.Name.ToLower(CultureInfo.InvariantCulture)))
                {
                    // this file is going to be deleted we can ignore it
                    continue;
                }

                // there is a file here that we cannot ignore
                return false;
            }

            return true;
        }


        /// <summary>
        /// purge a folder, removing any files that were automatically generated first
        /// </summary>
        /// <param name="folder">folder to delete</param>
        public void PurgeFolder(IDirectoryInfo folder)
        {
            IFileInfo[] files;
            try
            {
                files = folder.GetFiles("*.*");
            }
            catch (DirectoryNotFoundException)
            {
                // if the folder is not there then there is nothing to do
                return;
            }

            foreach (IFileInfo file in files)
            {
                if (_filesThatCanBeAutomaticallyDeleted.Contains(file.Name.ToLower(CultureInfo.InvariantCulture)))
                {
                    _fileUtilities.FileDelete(file.FullName);
                }
            }
            folder.Delete();
        }

    }
}
