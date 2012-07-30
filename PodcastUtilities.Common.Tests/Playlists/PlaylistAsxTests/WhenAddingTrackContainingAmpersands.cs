using NUnit.Framework;
using PodcastUtilities.Common.Playlists;

namespace PodcastUtilities.Common.Tests.Playlists.PlaylistAsxTests
{
    public class WhenAddingTrackContainingAmpersands
        : WhenTestingBehaviour
    {
        protected PlaylistAsx Playlist { get; set; }

        protected override void GivenThat()
        {
            base.GivenThat();

            Playlist = new PlaylistAsx("MyPodcastPlaylist.asx", true);
        }

        protected override void When()
        {
            Playlist.AddTrack(@"c:\podcasts\X & Y.wma");
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
            Assert.IsNotNull(Playlist.FindNode(@"ASX/ENTRY/REF[@HREF = 'c:\podcasts\X & Y.wma']"));
        }
    }
}