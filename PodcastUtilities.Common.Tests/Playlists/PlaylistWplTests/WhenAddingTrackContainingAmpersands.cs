using NUnit.Framework;
using PodcastUtilities.Common.Playlists;

namespace PodcastUtilities.Common.Tests.Playlists.PlaylistWplTests
{
    public class WhenAddingTrackContainingAmpersands
        : WhenTestingBehaviour
    {
        protected PlaylistWpl Playlist { get; set; }

        protected override void GivenThat()
        {
            base.GivenThat();

            Playlist = new PlaylistWpl("MyPodcastPlaylist.wpl", true);
        }

        protected override void When()
        {
            Playlist.AddTrack(@"c:\podcasts\A & B & C.wma");
        }

        [Test]
        public void ItShouldReturnTheCorrectNumberOfTracks()
        {
            Assert.AreEqual(1, Playlist.NumberOfTracks);
        }

        [Test]
        public void ItShouldInsertTheCorrectXml()
        {
            //Although the xml will be escaped before insertion, we should look for its original form
            Assert.IsNotNull(Playlist.FindNode(@"smil/body/seq/media[@src = 'c:\podcasts\A & B & C.wma']"));
        }
    }
}