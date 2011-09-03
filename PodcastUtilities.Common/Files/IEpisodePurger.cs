using System.Collections.Generic;
using PodcastUtilities.Common.Configuration;
using PodcastUtilities.Common.Platform;

namespace PodcastUtilities.Common.Files
{
    /// <summary>
    /// an object that can purge downloads
    /// </summary>
    public interface IEpisodePurger
    {
        /// <summary>
        /// find old downloads that can be deleted
        /// </summary>
        /// <param name="rootFolder">the root folder for all downloads</param>
        /// <param name="podcastInfo">info on the podcast to download</param>
        /// <returns>list of episodes to be deleted for the supplied podcastInfo</returns>
        IList<IFileInfo> FindEpisodesToPurge(string rootFolder, PodcastInfo podcastInfo);
    }
}