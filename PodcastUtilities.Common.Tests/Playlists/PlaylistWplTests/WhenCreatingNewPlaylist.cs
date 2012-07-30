using NUnit.Framework;
using PodcastUtilities.Common.Playlists;
using System.Xml;

namespace PodcastUtilities.Common.Tests.Playlists.PlaylistWplTests
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
			Assert.IsNotNull(Playlist.FindNode("smil/head"));
            Assert.IsNotNull(Playlist.FindNode("smil/body/seq"));
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
            var node = Playlist.FindNode("smil/head/title") as XmlNode;
            Assert.AreEqual("MyPodcastPlaylist", node.InnerText);
		}

		[Test]
		public void ItShouldHaveNoTracks()
		{
			Assert.AreEqual(0, Playlist.NumberOfTracks);
		}
	}
}