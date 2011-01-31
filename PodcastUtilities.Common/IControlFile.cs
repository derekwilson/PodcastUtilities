using System.Collections.Generic;

namespace PodcastUtilities.Common
{
	public interface IControlFile
	{
		string SourceRoot { get; }

		string DestinationRoot { get; }

		string PlaylistFilename { get; }

		PlaylistFormat PlaylistFormat { get; }

		long FreeSpaceToLeaveOnDestination { get; }

		IList<PodcastInfo> Podcasts { get; }
	}
}
