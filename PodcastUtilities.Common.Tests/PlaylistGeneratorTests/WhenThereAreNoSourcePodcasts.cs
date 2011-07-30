using NUnit.Framework;
using Rhino.Mocks;

namespace PodcastUtilities.Common.Tests.PlaylistGeneratorTests
{
    public class WhenThereAreNoSourcePodcasts : WhenTestingThePlaylistGenerator
    {
        protected override void When()
        {
            PlaylistGenerator.GeneratePlaylist(ControlFile,false);
        }

        [Test]
        public void ItShouldSaveTheFile()
        {
            Playlist.AssertWasCalled(p => p.SaveFile());
        }

        [Test]
        public void ItShouldNotAddTracks()
        {
            Playlist.AssertWasNotCalled(p => p.AddTrack(null), o => o.IgnoreArguments());
        }
    }
}
