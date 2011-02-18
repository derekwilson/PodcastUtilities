using System.Collections.Generic;
using PodcastUtilities.Common.IO;
using Rhino.Mocks;

namespace PodcastUtilities.Common.Tests.PlaylistGeneratorTests
{
    public abstract class WhenTestingThePlaylistGenerator
        : WhenTestingBehaviour
    {
        protected PlaylistGenerator PlaylistGenerator { get; set; }

        protected IFileUtilities FileUtilities { get; set; }
        protected IFileFinder Finder { get; set; }
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
            ControlFile.Stub(ctrl => ctrl.PlaylistFilename).Return("MyPodcasts.wpl");

            Finder = GenerateMock<IFileFinder>();
            FileUtilities = GenerateMock<IFileUtilities>();

            Playlist = GenerateMock<IPlaylist>();
            Factory = GenerateMock<IPlaylistFactory>();
            Factory.Stub(factory => factory.CreatePlaylist(PlaylistFormat.WPL, null)).IgnoreArguments().Return(Playlist);

            PlaylistGenerator = new PlaylistGenerator(Finder, FileUtilities, Factory);
        }
    }

}
