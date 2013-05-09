using NUnit.Framework;
using Rhino.Mocks;

namespace PodcastUtilities.Common.Tests.Playlists.GeneratorTests
{
	public class WhenCopyToDestinationIsSpecified : WhenTestingThePlaylistGenerator
	{
		protected override void When()
		{
			PlaylistGenerator.GeneratePlaylist(ControlFile, true);
		}

		[Test]
		public void ItShouldCopyTheFileToTheDestination()
		{
            Playlist.AssertWasCalled(p => p.SaveFile(@"c:\destination\MyPodcasts.wpl"));
		}
	}
}