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
using System;
using System.Collections.Generic;
using Moq;
using PodcastUtilities.Common.Configuration;
using PodcastUtilities.Common.Files;
using PodcastUtilities.Common.Platform;
using PodcastUtilities.Common.Playlists;

namespace PodcastUtilities.Common.Multiplatform.Tests.Playlists.GeneratorTests
{
    public abstract class WhenTestingThePlaylistGenerator
        : WhenTestingBehaviour
    {
        protected Generator PlaylistGenerator { get; set; }

        protected StatusUpdateEventArgs _statusUpdateArgs;

        protected Mock<IFileUtilities> FileUtilities { get; set; }
        protected Mock<IPathUtilities> PathUtilities { get; set; }
        protected Mock<IFinder> Finder { get; set; }
        protected Mock<IPlaylistFactory> Factory { get; set; }
        protected Mock<IPlaylist> Playlist { get; set; }
        protected Mock<IReadOnlyControlFile> ControlFile { get; set; }
        protected IList<IPodcastInfo> Podcasts { get; set; }

        protected override void GivenThat()
        {
            base.GivenThat();

            Podcasts = new List<IPodcastInfo>(10);

            ControlFile = GenerateMock<IReadOnlyControlFile>();
            ControlFile.Setup(ctrl => ctrl.GetSourceRoot()).Returns("c:\\source");
            ControlFile.Setup(ctrl => ctrl.GetDestinationRoot()).Returns("c:\\destination");
            ControlFile.Setup(ctrl => ctrl.GetPodcasts()).Returns(Podcasts);
            ControlFile.Setup(ctrl => ctrl.GetPlaylistFileName()).Returns("MyPodcasts.wpl");
            ControlFile.Setup(ctrl => ctrl.GetPlaylistPathSeparator()).Returns("||");

            Finder = GenerateMock<IFinder>();
            FileUtilities = GenerateMock<IFileUtilities>();
            PathUtilities = GenerateMock<IPathUtilities>();

            PathUtilities.Setup(utilities => utilities.GetPathSeparator()).Returns('\\');

            PathUtilities.Setup(utilities => utilities.GetFullPath("c:\\destination"))
                .Returns("c:\\destination");

            PathUtilities.Setup(utilities => utilities.GetTempFileName())
                .Returns("c:\\file.tmp");

            Playlist = GenerateMock<IPlaylist>();
            Factory = GenerateMock<IPlaylistFactory>();
            Factory.Setup(factory => factory.CreatePlaylist(It.IsAny<PlaylistFormat>(), It.IsAny<string>()))
                .Returns(Playlist.Object);

            PlaylistGenerator = new Generator(Finder.Object, FileUtilities.Object, PathUtilities.Object, Factory.Object);

            PlaylistGenerator.StatusUpdate += new EventHandler<StatusUpdateEventArgs>(GenerateStatusUpdate);
        }

        private void GenerateStatusUpdate(object sender, StatusUpdateEventArgs e)
        {
            _statusUpdateArgs = e;
        }
    }

}
