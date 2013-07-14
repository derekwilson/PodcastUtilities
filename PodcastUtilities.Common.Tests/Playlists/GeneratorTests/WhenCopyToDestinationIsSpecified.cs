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
        public void ItShouldSaveTheTemporaryFile()
        {
            Playlist.AssertWasCalled(p => p.SaveFile(@"c:\file.tmp"));
        }

        [Test]
        public void ItShouldCopyThePlaylistToTheCorrectLocation()
        {
            FileUtilities.AssertWasCalled(utilities => utilities.FileCopy(@"c:\file.tmp", @"c:\destination\MyPodcasts.wpl", true));
        }
	}
}