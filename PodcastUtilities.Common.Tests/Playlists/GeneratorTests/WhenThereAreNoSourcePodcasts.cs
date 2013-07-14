using NUnit.Framework;
using Rhino.Mocks;

namespace PodcastUtilities.Common.Tests.Playlists.GeneratorTests
{
    public class WhenThereAreNoSourcePodcasts : WhenTestingThePlaylistGenerator
    {
        protected override void When()
        {
            PlaylistGenerator.GeneratePlaylist(ControlFile,false);
        }

        [Test]
        public void ItShouldSaveTheTemporaryFile()
        {
            Playlist.AssertWasCalled(p => p.SaveFile(@"c:\file.tmp"));
        }

        [Test]
        public void ItShouldNotAddTracks()
        {
            Playlist.AssertWasNotCalled(p => p.AddTrack(null), o => o.IgnoreArguments());
        }

        [Test]
        public void ItShouldCopyThePlaylistToTheCorrectLocation()
        {
            FileUtilities.AssertWasCalled(utilities => utilities.FileCopy(@"c:\file.tmp", "MyPodcasts.wpl", true));
        }
    }
}
