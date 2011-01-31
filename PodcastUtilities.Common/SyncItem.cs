using PodcastUtilities.Common.IO;

namespace PodcastUtilities.Common
{
	public class SyncItem
	{
		public IFileInfo Source { get; set; }
		public string DestinationPath { get; set; }
		public bool Copied { get; set; }
	}
}
