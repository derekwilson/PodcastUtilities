using System;
using NUnit.Framework;

namespace PodcastUtilities.Common.Tests.PlaylistFactoryTests
{
    public abstract class WhenCreatingANewPlaylist
        : WhenTestingBehaviour
    {
        protected PlaylistFormat PlayListFormat { get; set; }
        protected IPlaylist Playlist { get; set; }
        protected IPlaylistFactory PlaylistFactory { get; set; }
        protected string PlayllistFilename { get; set; }

        protected override void GivenThat()
        {
            base.GivenThat();
            PlaylistFactory = new PlaylistFactory();
            PlayllistFilename = "myplaylist.ext";
        }

        protected override void When()
        {
            Playlist = PlaylistFactory.CreatePlaylist(PlayListFormat, PlayllistFilename);
        }
    }

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
