using NUnit.Framework;
using PodcastUtilities.Common.Playlists;

namespace PodcastUtilities.Common.Tests.Playlists.PlaylistWplTests
{
	public class WhenAddingTrackThatIsAlreadyInPlaylist
		: WhenTestingBehaviour
	{
		protected PlaylistWpl Playlist { get; set; }

		protected bool Added { get; set; }

		protected override void GivenThat()
		{
			base.GivenThat();

			Playlist = new PlaylistWpl("MyPodcastPlaylist.wpl", true);
			Playlist.AddTrack(@"c:\podcasts\1.mp3");
		}

		protected override void When()
		{
			Added = Playlist.AddTrack(@"c:\podcasts\1.mp3");
		}

		[Test]
		public void ItShouldReturnFalseFromAddTrack()
		{
			Assert.IsFalse(Added);
		}

		[Test]
		public void ItShouldNotInsertTheXmlTwice()
		{
			var podcastNodes = Playlist.GetNumberOfNodes(@"smil/body/seq/media[@src = 'c:\podcasts\1.mp3']");

			Assert.AreEqual(1, podcastNodes);
		}
	}
}