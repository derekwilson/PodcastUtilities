using System.Xml;
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
			Assert.IsNotNull(Playlist.FindNode("ASX/TITLE"));
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
		    var node = Playlist.FindNode("ASX/TITLE") as XmlNode;
			Assert.AreEqual("MyPodcastPlaylist", node.InnerText);
		}

		[Test]
		public void ItShouldHaveNoTracks()
		{
			Assert.AreEqual(0, Playlist.NumberOfTracks);
		}
	}
}