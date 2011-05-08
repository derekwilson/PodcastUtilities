using PodcastUtilities.Common.IO;

namespace PodcastUtilities.Common
{
	/// <summary>
	/// an individulal item to be synchronised
	/// </summary>
    public class SyncItem
	{
        /// <summary>
        /// the item in the file system to be synchronised
        /// </summary>
        public IFileInfo Source { get; set; }
        /// <summary>
        /// pathname to be copied to
        /// </summary>
		public string DestinationPath { get; set; }
        /// <summary>
        /// true if it has been copied
        /// </summary>
		public bool Copied { get; set; }
	}
}
