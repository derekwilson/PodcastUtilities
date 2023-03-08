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
using Moq;
using NUnit.Framework;
using PodcastUtilities.Common.Configuration;
using PodcastUtilities.Common.Platform;

namespace PodcastUtilities.Common.Multiplatform.Tests.Playlists.GeneratorTests
{
    public class WhenTheStatusUpdates : WhenTestingThePlaylistGenerator
    {
        protected Mock<IFileInfo> _file1;
        protected Mock<IFileInfo> _file2;
        protected Mock<IFileInfo> _file3;
        protected Mock<IFileInfo> _file4;
        protected Mock<IFileInfo> _file5;

        protected override void GivenThat()
        {
            base.GivenThat();

            Podcasts.Clear();
            Podcasts.Add(new PodcastInfo(ControlFile.Object) { Folder = "Hanselminutes" });
            Podcasts.Add(new PodcastInfo(ControlFile.Object) { Folder = "This Developers Life" });
            Podcasts[0].Pattern.Value = "*.mp3";
            Podcasts[1].Pattern.Value = "*.wma";

            _file1 = GenerateMock<IFileInfo>();
            _file2 = GenerateMock<IFileInfo>();
            _file3 = GenerateMock<IFileInfo>();
            _file4 = GenerateMock<IFileInfo>();
            _file5 = GenerateMock<IFileInfo>();

            var podcastFiles1 = new List<IFileInfo> { _file1.Object, _file2.Object };
            _file1.Setup(f => f.FullName).Returns(@"c:\destination\Hanselminutes\001.mp3");
            _file2.Setup(f => f.FullName).Returns(@"c:\destination\Hanselminutes\002.mp3");

            var podcastFiles2 = new List<IFileInfo> { _file3.Object, _file4.Object, _file5.Object };
            _file3.Setup(f => f.FullName).Returns(@"c:\destination\This Developers Life\997.wma");
            _file4.Setup(f => f.FullName).Returns(@"c:\destination\This Developers Life\998.wma");
            _file5.Setup(f => f.FullName).Returns(@"c:\destination\This Developers Life\999.wma");

            Finder.Setup(f => f.GetFiles(@"c:\destination\Hanselminutes", "*.mp3"))
                .Returns(podcastFiles1);

            Finder.Setup(f => f.GetFiles(@"c:\destination\This Developers Life", "*.wma"))
                .Returns(podcastFiles2);
        }

        protected override void When()
        {
            PlaylistGenerator.GeneratePlaylist(ControlFile.Object, false);
        }

        [Test]
        public void ItShouldUpdateTheStatus()
        {
            Assert.That(_statusUpdateArgs.Message, Is.EqualTo("Writing playlist to MyPodcasts.wpl"));
            Assert.That(_statusUpdateArgs.MessageLevel, Is.EqualTo(StatusUpdateLevel.Status));
            Assert.That(_statusUpdateArgs.IsTaskCompletedSuccessfully, Is.EqualTo(true));
            Assert.That(_statusUpdateArgs.UserState, Is.SameAs(Playlist.Object));
        }
    }
}