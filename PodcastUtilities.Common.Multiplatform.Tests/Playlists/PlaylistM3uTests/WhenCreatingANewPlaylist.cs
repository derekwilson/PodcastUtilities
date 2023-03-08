using NUnit.Framework;
using PodcastUtilities.Common.Playlists;

namespace PodcastUtilities.Common.Multiplatform.Tests.Playlists.PlaylistM3uTests
{
    public class WhenCreatingANewPlaylist : WhenTestingBehaviour
    {
        protected PlaylistM3u Playlist { get; set; }

        protected override void When()
        {
            Playlist = new PlaylistM3u("MyPodcastPlaylist.m3u", true);
        }

        [Test]
        public void ItShouldSetTheFilename()
        {
            Assert.AreEqual("MyPodcastPlaylist.m3u", Playlist.FileName);
        }

        [Test]
        public void ItShouldSetTheTitle()
        {
            Assert.AreEqual("MyPodcastPlaylist", Playlist.Title);
        }

        [Test]
        public void ItShouldHaveNoTracks()
        {
            Assert.AreEqual(0, Playlist.NumberOfTracks);
        }
    }
}
