using System.Collections.Generic;
using PodcastUtilities.Common.Configuration;
using PodcastUtilities.Common.Files;
using PodcastUtilities.Common.Platform;
using PodcastUtilities.Common.Playlists;
using Rhino.Mocks;

namespace PodcastUtilities.Common.Tests.PlaylistGeneratorTests
{
    public abstract class WhenTestingThePlaylistGenerator
        : WhenTestingBehaviour
    {
        protected Generator PlaylistGenerator { get; set; }

        protected IFileUtilities FileUtilities { get; set; }
        protected IFinder Finder { get; set; }
        protected IPlaylistFactory Factory { get; set; }
        protected IPlaylist Playlist { get; set; }
        protected IControlFile ControlFile { get; set; }
        protected IList<PodcastInfo> Podcasts { get; set; }

        protected override void GivenThat()
        {
            base.GivenThat();

            Podcasts = new List<PodcastInfo>(10);

            ControlFile = GenerateMock<IControlFile>();
            ControlFile.Stub(ctrl => ctrl.SourceRoot).Return("c:\\source");
            ControlFile.Stub(ctrl => ctrl.DestinationRoot).Return("c:\\destination");
            ControlFile.Stub(ctrl => ctrl.Podcasts).Return(Podcasts);
            ControlFile.Stub(ctrl => ctrl.PlaylistFileName).Return("MyPodcasts.wpl");

            Finder = GenerateMock<IFinder>();
            FileUtilities = GenerateMock<IFileUtilities>();

            Playlist = GenerateMock<IPlaylist>();
            Factory = GenerateMock<IPlaylistFactory>();
            Factory.Stub(factory => factory.CreatePlaylist(PlaylistFormat.WPL, null)).IgnoreArguments().Return(Playlist);

            PlaylistGenerator = new Generator(Finder, FileUtilities, Factory);
        }
    }

}
