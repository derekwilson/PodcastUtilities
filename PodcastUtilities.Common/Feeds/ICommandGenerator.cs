using System;
using PodcastUtilities.Common.Configuration;

namespace PodcastUtilities.Common.Feeds
{
    /// <summary>
    /// generates full commands from the provided podcast and command template
    /// </summary>
    public interface ICommandGenerator
    {
        /// <summary>
        /// replace any token elements in a command line and return a full command ready to be executed
        /// </summary>
        /// <param name="commandLine">tokenised command line</param>
        /// <param name="rootFolder">the root folder for all downloads</param>
        /// <param name="destinationPathname">the full pathname for the download</param>
        /// <param name="podcast">the podcast, used as a source of tokens</param>
        /// <returns>a full command line, NULL if there is no command to execute</returns>
        IExternalCommand ReplaceTokensInCommandline(string commandLine, string rootFolder, string destinationPathname, PodcastInfo podcast);
    }
}