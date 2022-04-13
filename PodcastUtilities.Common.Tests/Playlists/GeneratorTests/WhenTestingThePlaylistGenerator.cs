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
using PodcastUtilities.Common.Configuration;
using PodcastUtilities.Common.Files;
using PodcastUtilities.Common.Platform;
using PodcastUtilities.Common.Playlists;
using Rhino.Mocks;

namespace PodcastUtilities.Common.Tests.Playlists.GeneratorTests
{
    public abstract class WhenTestingThePlaylistGenerator
        : WhenTestingBehaviour
    {
        protected Generator PlaylistGenerator { get; set; }

        protected IFileUtilities FileUtilities { get; set; }
        protected IPathUtilities PathUtilities { get; set; }
        protected IFinder Finder { get; set; }
        protected IPlaylistFactory Factory { get; set; }
        protected IPlaylist Playlist { get; set; }
        protected IReadOnlyControlFile ControlFile { get; set; }
        protected IList<PodcastInfo> Podcasts { get; set; }

        protected override void GivenThat()
        {
            base.GivenThat();

            Podcasts = new List<PodcastInfo>(10);

            ControlFile = GenerateMock<IReadOnlyControlFile>();
            ControlFile.Stub(ctrl => ctrl.GetSourceRoot()).Return("c:\\source");
            ControlFile.Stub(ctrl => ctrl.GetDestinationRoot()).Return("c:\\destination");
            ControlFile.Stub(ctrl => ctrl.GetPodcasts()).Return(Podcasts);
			ControlFile.Stub(ctrl => ctrl.GetPlaylistFileName()).Return("MyPodcasts.wpl");
			ControlFile.Stub(ctrl => ctrl.GetPlaylistPathSeparator()).Return("||");

			Finder = GenerateMock<IFinder>();
            FileUtilities = GenerateMock<IFileUtilities>();
            PathUtilities = GenerateMock<IPathUtilities>();

			PathUtilities.Stub(utilities => utilities.GetPathSeparator()).Return('\\');

            PathUtilities.Stub(utilities => utilities.GetFullPath("c:\\destination"))
                .Return("c:\\destination");

            PathUtilities.Stub(utilities => utilities.GetTempFileName())
                .Return("c:\\file.tmp");

            if (Playlist == null)
            {
                Playlist = GenerateMock<IPlaylist>();
            }
            Factory = GenerateMock<IPlaylistFactory>();
            Factory.Stub(factory => factory.CreatePlaylist(PlaylistFormat.WPL, null)).IgnoreArguments().Return(Playlist);

            PlaylistGenerator = new Generator(Finder, FileUtilities, PathUtilities, Factory);
        }
    }

}
