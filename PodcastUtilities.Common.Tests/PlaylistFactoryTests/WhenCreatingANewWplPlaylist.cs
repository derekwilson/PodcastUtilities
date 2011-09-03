using NUnit.Framework;
using PodcastUtilities.Common.Playlists;

namespace PodcastUtilities.Common.Tests.PlaylistFactoryTests
{
    public class WhenCreatingANewWplPlaylist
        : WhenCreatingANewPlaylist
    {
        protected override void GivenThat()
        {
            base.GivenThat();
            PlayListFormat = PlaylistFormat.WPL;
        }

        [Test]
        public void ItShouldReturnTheCorrectObject()
        {
            Assert.That(Playlist, Is.InstanceOf<PlaylistWpl>());
        }

        [Test]
        public void ItShouldSetTheTitle()
        {
            Assert.AreEqual("myplaylist", Playlist.Title);
        }
    }
}