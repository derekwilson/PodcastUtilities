using System.Collections.Generic;

namespace PodcastUtilities.Common
{
    /// <summary>
    /// this object represents the xml control file
    /// </summary>
    public interface IControlFile
	{
        /// <summary>
        /// pathname to the root folder to copy from when synchronising
        /// </summary>
        string SourceRoot { get; }

        /// <summary>
        /// pathname to the destination root folder
        /// </summary>
        string DestinationRoot { get; }

        /// <summary>
        /// filename and extension for the generated playlist
        /// </summary>
        string PlaylistFilename { get; }

        /// <summary>
        /// the format for the generated playlist
        /// </summary>
        PlaylistFormat PlaylistFormat { get; }

        /// <summary>
        /// free space in MB to leave on the destination device
        /// </summary>
        long FreeSpaceToLeaveOnDestination { get; }

        /// <summary>
        /// the configuration for the individual podcasts
        /// </summary>
        IList<PodcastInfo> Podcasts { get; }
	}
}
