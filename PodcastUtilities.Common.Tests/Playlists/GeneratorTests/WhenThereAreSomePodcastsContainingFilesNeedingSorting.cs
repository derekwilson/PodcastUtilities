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
using System.Collections.Generic;
using NUnit.Framework;
using PodcastUtilities.Common.Configuration;
using PodcastUtilities.Common.Platform;
using PodcastUtilities.Common.Playlists;
using Rhino.Mocks;

namespace PodcastUtilities.Common.Tests.Playlists.GeneratorTests
{
	public class WhenThereAreSomePodcastsContainingFilesNeedingSorting : WhenTestingThePlaylistGenerator
	{
        protected MockRepository mocks = new MockRepository();

        protected override void GivenThat()
		{
            Playlist = mocks.DynamicMock<IPlaylist>();

			base.GivenThat();

            Podcasts.Clear();
			Podcasts.Add(new PodcastInfo(ControlFile) { Folder = "Hanselminutes" });
			Podcasts.Add(new PodcastInfo(ControlFile) { Folder = "This Developers Life" });
            Podcasts[0].Pattern.Value = "*.mp3";
            Podcasts[1].Pattern.Value = "*.wma";

			var podcastFiles1 = new List<IFileInfo> {GenerateMock<IFileInfo>(), GenerateMock<IFileInfo>()};
			podcastFiles1[0].Stub(f => f.FullName).Return(@"c:\destination\Hanselminutes\001.mp3");
			podcastFiles1[1].Stub(f => f.FullName).Return(@"c:\destination\Hanselminutes\002.mp3");

			var podcastFiles2 = new List<IFileInfo> {GenerateMock<IFileInfo>(), GenerateMock<IFileInfo>(), GenerateMock<IFileInfo>()};
            // add them so they need sorting
            podcastFiles2[0].Stub(f => f.FullName).Return(@"c:\destination\This Developers Life\997.wma");
			podcastFiles2[1].Stub(f => f.FullName).Return(@"c:\destination\This Developers Life\999.wma");
			podcastFiles2[2].Stub(f => f.FullName).Return(@"c:\destination\This Developers Life\998.wma");

			Finder.Stub(f => f.GetFiles(@"c:\destination\Hanselminutes", "*.mp3"))
				.Return(podcastFiles1);

			Finder.Stub(f => f.GetFiles(@"c:\destination\This Developers Life", "*.wma"))
				.Return(podcastFiles2);

            using (mocks.Ordered())
            {
                Playlist.Expect(x => x.AddTrack(@".||Hanselminutes||001.mp3")).Return(true);
                Playlist.Expect(x => x.AddTrack(@".||Hanselminutes||002.mp3")).Return(true);
                Playlist.Expect(x => x.AddTrack(@".||This Developers Life||997.wma")).Return(true);
                Playlist.Expect(x => x.AddTrack(@".||This Developers Life||998.wma")).Return(true);
                Playlist.Expect(x => x.AddTrack(@".||This Developers Life||999.wma")).Return(true);
            }
            Playlist.Replay();
        }

        protected override void When()
		{
			PlaylistGenerator.GeneratePlaylist(ControlFile,false);
		}


        [Test]
        public void ItShouldAddAllTheTracksForEachPodcastInTheCorrectOrder()
        {
            Playlist.VerifyAllExpectations();
        }

    }
}