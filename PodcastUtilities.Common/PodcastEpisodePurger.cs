using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using PodcastUtilities.Common.Platform;

namespace PodcastUtilities.Common
{
    /// <summary>
    /// purge old download files
    /// </summary>
    public class PodcastEpisodePurger : IPodcastEpisodePurger
    {
        private IDirectoryInfoProvider _directoryInfoProvider;
        private readonly IFileUtilities _fileUtilities;
        private readonly ITimeProvider _timeProvider;

        /// <summary>
        /// create the purger
        /// </summary>
        public PodcastEpisodePurger(IFileUtilities fileUtilities, ITimeProvider timeProvider, IDirectoryInfoProvider directoryInfoProvider)
        {
            _fileUtilities = fileUtilities;
            _directoryInfoProvider = directoryInfoProvider;
            _timeProvider = timeProvider;
        }

        private DateTime GetWhenDownloadWasPublished(PodcastInfo podcastInfo, IFileInfo file)
        {
            switch (podcastInfo.Feed.NamingStyle)
            {
                case PodcastEpisodeNamingStyle.UrlFilenameAndPublishDateTime:
                case PodcastEpisodeNamingStyle.UrlFilenameFeedTitleAndPublishDateTime:
                case PodcastEpisodeNamingStyle.EpisodeTitleAndPublishDateTime:
                case PodcastEpisodeNamingStyle.UrlFilenameFeedTitleAndPublishDateTimeInFolder:
                    try
                    {
                        return ConvertFilenameToPublishedDate(Path.GetFileNameWithoutExtension(file.FullName));
                    }
                    catch (Exception)
                    {
                        return file.CreationTime;
                    }
                case PodcastEpisodeNamingStyle.EpisodeTitle:
                case PodcastEpisodeNamingStyle.UrlFilename:
                    return file.CreationTime;
                default:
                    throw new ArgumentOutOfRangeException("NamingStyle");
            }
        }

        private DateTime ConvertFilenameToPublishedDate(string fileName)
        {
            return new DateTime(
                    Convert.ToInt32(fileName.Substring(0, 4)),
                    Convert.ToInt32(fileName.Substring(5, 2)),
                    Convert.ToInt32(fileName.Substring(8, 2)),
                    Convert.ToInt32(fileName.Substring(11, 2)),
                    Convert.ToInt32(fileName.Substring(13, 2)),
                    0
                );
        }


        /// <summary>
        /// find old downloads that can be deleted
        /// </summary>
        /// <param name="rootFolder">the root folder for all downloads</param>
        /// <param name="podcastInfo">info on the podcast to download</param>
        /// <returns>list of episodes to be deleted for the supplied podcastInfo</returns>
        public IList<IFileInfo> FindEpisodesToPurge(string rootFolder, PodcastInfo podcastInfo)
        {
            List<IFileInfo> episodesToDelete = new List<IFileInfo>(10);
            if (podcastInfo.Feed == null)
            {
                // it is optional to have a feed
                return episodesToDelete;
            }

            var feedDownloadsFolder = Path.Combine(rootFolder, podcastInfo.Folder);
            var oldestEpisodeToKeep = DateTime.MinValue;
            if (podcastInfo.Feed.DeleteDownloadsDaysOld < int.MaxValue)
            {
                oldestEpisodeToKeep = _timeProvider.UtcNow.AddDays(-podcastInfo.Feed.DeleteDownloadsDaysOld);
            }

            if (IsSubFolderBasedNaming(podcastInfo.Feed.NamingStyle))
            {
                ScanSubFoldersForOldFiles(feedDownloadsFolder, oldestEpisodeToKeep, episodesToDelete, podcastInfo);
            }
            else
            {
                ScanFolderForOldFiles(feedDownloadsFolder,oldestEpisodeToKeep,episodesToDelete,podcastInfo);
            }

            return episodesToDelete;
        }

        private bool IsSubFolderBasedNaming(PodcastEpisodeNamingStyle style)
        {
            return style == PodcastEpisodeNamingStyle.UrlFilenameFeedTitleAndPublishDateTimeInFolder;
        }

        private void ScanSubFoldersForOldFiles(string folderToScan, DateTime oldestEpisodeToKeep, List<IFileInfo> episodesToDelete, PodcastInfo podcastInfo)
        {
            var directoryInfo = _directoryInfoProvider.GetDirectoryInfo(folderToScan);

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

            foreach (var subFolder in subFolders)
            {
                ScanFolderForOldFiles(subFolder.FullName,oldestEpisodeToKeep,episodesToDelete,podcastInfo);
            }
        }

        private void ScanFolderForOldFiles(string folderToScan, DateTime oldestEpisodeToKeep, List<IFileInfo> episodesToDelete, PodcastInfo podcastInfo)
        {
            var directoryInfo = _directoryInfoProvider.GetDirectoryInfo(folderToScan);

            IFileInfo[] files;
            try
            {
                files = directoryInfo.GetFiles(podcastInfo.Pattern);
            }
            catch (DirectoryNotFoundException)
            {
                // if the folder is not there then there is nothing to do
                return;
            }

            foreach (var file in files)
            {
                var extension = Path.GetExtension(file.FullName);
                if (extension != null && extension.ToLower() == ".xml")
                {
                    // do not delete the state file
                    continue;
                }
                if (GetWhenDownloadWasPublished(podcastInfo, file) < oldestEpisodeToKeep)
                    episodesToDelete.Add(file);
            }
        }
    }
}
