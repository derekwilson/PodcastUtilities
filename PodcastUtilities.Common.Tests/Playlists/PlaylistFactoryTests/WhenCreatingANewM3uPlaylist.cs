using NUnit.Framework;
using PodcastUtilities.Common.Playlists;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PodcastUtilities.Common.Tests.Playlists.PlaylistFactoryTests
{
	public class WhenCreatingANewM3uPlaylist : WhenCreatingANewPlaylist
	{
		protected override void GivenThat()
		{
			base.GivenThat();
			PlayListFormat = PlaylistFormat.M3U;
		}

		[Test]
		public void ItShouldReturnTheCorrectObject()
		{
			Assert.That(Playlist, Is.InstanceOf<PlaylistM3u>());
		}

		[Test]
		public void ItShouldSetTheTitle()
		{
			Assert.AreEqual("myplaylist", Playlist.Title);
		}
	}
}
