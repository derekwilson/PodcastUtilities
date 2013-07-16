#region License
// FreeBSD License
// Copyright (c) 2010 - 2013, Andrew Trevarrow and Derek Wilson
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
// 
// Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
// 
// Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED 
// WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A 
// PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR
// ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED
// TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING 
// NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
// POSSIBILITY OF SUCH DAMAGE.
#endregion
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