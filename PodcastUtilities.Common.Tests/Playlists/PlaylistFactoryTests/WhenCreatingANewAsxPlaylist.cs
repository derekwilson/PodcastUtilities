using NUnit.Framework;
using PodcastUtilities.Common.Playlists;

namespace PodcastUtilities.Common.Tests.Playlists.PlaylistFactoryTests
{
    public class WhenCreatingANewAsxPlaylist
        : WhenCreatingANewPlaylist
    {
        protected override void GivenThat()
        {
            base.GivenThat();
            PlayListFormat = PlaylistFormat.ASX;
        }

        [Test]
        public void ItShouldReturnTheCorrectObject()
        {
            Assert.That(Playlist, Is.InstanceOf<PlaylistAsx>());
        }

        [Test]
        public void ItShouldSetTheTitle()
        {
            Assert.AreEqual("myplaylist", Playlist.Title);
        }
    }
}