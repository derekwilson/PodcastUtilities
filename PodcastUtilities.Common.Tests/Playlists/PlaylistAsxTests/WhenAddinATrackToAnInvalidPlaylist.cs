using System;
using NUnit.Framework;
using PodcastUtilities.Common.Playlists;

namespace PodcastUtilities.Common.Tests.Playlists.PlaylistAsxTests
{
    public class WhenAddinATrackToAnInvalidPlaylist : WhenTestingBehaviour
    {
        protected PlaylistAsx Playlist { get; set; }
        protected Exception ThrownException { get; set; }

        protected override void GivenThat()
        {
            base.GivenThat();

            Playlist = new PlaylistAsx("MyPodcastPlaylist.asx", true);
            Playlist.LoadXml("<xyz></xyz>");
        }

        protected override void When()
        {
            ThrownException = null;
            try
            {
                Playlist.AddTrack(@"c:\podcasts\1.mp3");
            }
            catch (Exception exception)
            {
                ThrownException = exception;
            }
        }

        [Test]
        public void ItShouldThrow()
        {
            Assert.IsInstanceOf(typeof(Exception), ThrownException);
        }
    }
}