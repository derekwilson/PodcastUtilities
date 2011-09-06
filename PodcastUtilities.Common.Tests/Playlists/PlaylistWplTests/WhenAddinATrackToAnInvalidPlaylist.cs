using System;
using System.Xml;
using NUnit.Framework;
using PodcastUtilities.Common.Playlists;

namespace PodcastUtilities.Common.Tests.Playlists.PlaylistWplTests
{
    public class WhenAddinATrackToAnInvalidPlaylist : WhenTestingBehaviour
    {
        protected PlaylistWpl Playlist { get; set; }
        protected Exception ThrownException { get; set; }

        protected override void GivenThat()
        {
            base.GivenThat();

            Playlist = new PlaylistWpl("MyPodcastPlaylist.wpl", true);
            var node = Playlist.FindNode(@"smil/body") as XmlNode;
            node.RemoveAll();
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