using NUnit.Framework;
using PodcastUtilities.Common.Playlists;

namespace PodcastUtilities.Common.Tests.Playlists.PlaylistAsxTests
{
	public class WhenAddingTracksThatAreNotAlreadyInPlaylist
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
			Playlist.AddTrack(@"c:\podcasts\1.mp3");
			Playlist.AddTrack(@"c:\podcasts\2.mp3");
		}

		[Test]
		public void ItShouldReturnTheCorrectNumberOfTracks()
		{
			Assert.AreEqual(2, Playlist.NumberOfTracks);
		}

		[Test]
		public void ItShouldInsertTheCorrectXml()
		{
			Assert.IsNotNull(Playlist.SelectSingleNode(@"ASX/ENTRY/REF[@HREF = 'c:\podcasts\1.mp3']"));
			Assert.IsNotNull(Playlist.SelectSingleNode(@"ASX/ENTRY/REF[@HREF = 'c:\podcasts\2.mp3']"));
		}
	}
}