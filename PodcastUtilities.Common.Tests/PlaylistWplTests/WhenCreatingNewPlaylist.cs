using NUnit.Framework;
using PodcastUtilities.Common.Playlists;

namespace PodcastUtilities.Common.Tests.PlaylistWplTests
{
	public class WhenCreatingNewPlaylist
		: WhenTestingBehaviour
	{
		protected PlaylistWpl Playlist { get; set; }

		protected override void When()
		{
			Playlist = new PlaylistWpl("MyPodcastPlaylist.wpl", true);
		}

		[Test]
		public void ItShouldLoadTheEmptyPlaylistResource()
		{
			Assert.IsNotNull(Playlist.SelectSingleNode("smil/head"));
			Assert.IsNotNull(Playlist.SelectSingleNode("smil/body/seq"));
		}

		[Test]
		public void ItShouldSetTheFilename()
		{
			Assert.AreEqual("MyPodcastPlaylist.wpl", Playlist.FileName);
		}

		[Test]
		public void ItShouldSetTheTitle()
		{
			Assert.AreEqual("MyPodcastPlaylist", Playlist.Title);
			Assert.AreEqual("MyPodcastPlaylist", Playlist.SelectSingleNode("smil/head/title").InnerText);
		}

		[Test]
		public void ItShouldHaveNoTracks()
		{
			Assert.AreEqual(0, Playlist.NumberOfTracks);
		}
	}
}