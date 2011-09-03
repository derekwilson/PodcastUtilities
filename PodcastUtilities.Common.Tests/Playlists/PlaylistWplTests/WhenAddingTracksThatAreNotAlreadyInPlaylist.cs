using NUnit.Framework;
using PodcastUtilities.Common.Playlists;

namespace PodcastUtilities.Common.Tests.Playlists.PlaylistWplTests
{
	public class WhenAddingTracksThatAreNotAlreadyInPlaylist
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
			Playlist.AddTrack(@"c:\podcasts\1.mp3");
			Playlist.AddTrack(@"c:\podcasts\2.mp3");
			Playlist.AddTrack(@"c:\podcasts\3.wma");
		}

		[Test]
		public void ItShouldReturnTheCorrectNumberOfTracks()
		{
			Assert.AreEqual(3, Playlist.NumberOfTracks);
		}

		[Test]
		public void ItShouldInsertTheCorrectXml()
		{
			Assert.IsNotNull(Playlist.SelectSingleNode(@"smil/body/seq/media[@src = 'c:\podcasts\1.mp3']"));
			Assert.IsNotNull(Playlist.SelectSingleNode(@"smil/body/seq/media[@src = 'c:\podcasts\2.mp3']"));
			Assert.IsNotNull(Playlist.SelectSingleNode(@"smil/body/seq/media[@src = 'c:\podcasts\3.wma']"));
		}
	}
}