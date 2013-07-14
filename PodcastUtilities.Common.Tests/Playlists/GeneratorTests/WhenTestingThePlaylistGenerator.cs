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

            Finder = GenerateMock<IFinder>();
            FileUtilities = GenerateMock<IFileUtilities>();
            PathUtilities = GenerateMock<IPathUtilities>();

            PathUtilities.Stub(utilities => utilities.GetFullPath("c:\\destination"))
                .Return("c:\\destination");

            PathUtilities.Stub(utilities => utilities.GetTempFileName())
                .Return("c:\\file.tmp");

            Playlist = GenerateMock<IPlaylist>();
            Factory = GenerateMock<IPlaylistFactory>();
            Factory.Stub(factory => factory.CreatePlaylist(PlaylistFormat.WPL, null)).IgnoreArguments().Return(Playlist);

            PlaylistGenerator = new Generator(Finder, FileUtilities, PathUtilities, Factory);
        }
    }

}
