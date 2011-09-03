using NUnit.Framework;
using PodcastUtilities.Common.Playlists;

namespace PodcastUtilities.Common.Tests.Playlists.PlaylistAsxTests
{
	public class WhenCreatingNewPlaylist
		: WhenTestingBehaviour
	{
		protected PlaylistAsx Playlist { get; set; }

		protected override void When()
		{
			Playlist = new PlaylistAsx("MyPodcastPlaylist.asx", true);
		}

		[Test]
		public void ItShouldLoadTheEmptyPlaylistResource()
		{
			Assert.IsNotNull(Playlist.SelectSingleNode("ASX/TITLE"));
		}

		[Test]
		public void ItShouldSetTheFilename()
		{
			Assert.AreEqual("MyPodcastPlaylist.asx", Playlist.FileName);
		}

		[Test]
		public void ItShouldSetTheTitle()
		{
			Assert.AreEqual("MyPodcastPlaylist", Playlist.Title);
			Assert.AreEqual("MyPodcastPlaylist", Playlist.SelectSingleNode("ASX/TITLE").InnerText);
		}

		[Test]
		public void ItShouldHaveNoTracks()
		{
			Assert.AreEqual(0, Playlist.NumberOfTracks);
		}
	}
}