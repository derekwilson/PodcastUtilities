using System;
using NUnit.Framework;
using PodcastUtilities.Common.Playlists;

namespace PodcastUtilities.Common.Tests.PlaylistFactoryTests
{
    public class WhenCreatingANewUnknownPlaylist
        : WhenCreatingANewPlaylist
    {
        protected override void GivenThat()
        {
            base.GivenThat();
            PlayListFormat = PlaylistFormat.Unknown;
        }

        [Test]
        public void ItShouldReturnTheCorrectObject()
        {
            Assert.That(Playlist, Is.Null);
        }

        [Test]
        public void ItShouldSetTheTitle()
        {
            Assert.That(ThrownException, Is.InstanceOf<ArgumentOutOfRangeException>());
        }
    }
}