using System.Collections.Generic;
using NUnit.Framework;
using PodcastUtilities.Common.Configuration;
using PodcastUtilities.Common.Platform;
using Rhino.Mocks;

namespace PodcastUtilities.Common.Tests.Playlists.GeneratorTests
{
	public class WhenThereAreSomePodcastsContainingFiles : WhenTestingThePlaylistGenerator
	{
		protected override void GivenThat()
		{
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
			podcastFiles2[0].Stub(f => f.FullName).Return(@"c:\destination\This Developers Life\997.wma");
			podcastFiles2[1].Stub(f => f.FullName).Return(@"c:\destination\This Developers Life\998.wma");
			podcastFiles2[2].Stub(f => f.FullName).Return(@"c:\destination\This Developers Life\999.wma");

			Finder.Stub(f => f.GetFiles(@"c:\destination\Hanselminutes", "*.mp3"))
				.Return(podcastFiles1);

			Finder.Stub(f => f.GetFiles(@"c:\destination\This Developers Life", "*.wma"))
				.Return(podcastFiles2);
		}

		protected override void When()
		{
			PlaylistGenerator.GeneratePlaylist(ControlFile,false);
		}

		[Test]
        public void ItShouldSaveTheTemporaryFile()
		{
            Playlist.AssertWasCalled(p => p.SaveFile(@"c:\file.tmp"));
		}

		[Test]
		public void ItShouldAddAllTheTracksForEachPodcast()
		{
			Playlist.AssertWasCalled(p => p.AddTrack(@".\Hanselminutes\001.mp3"));
			Playlist.AssertWasCalled(p => p.AddTrack(@".\Hanselminutes\002.mp3"));
			Playlist.AssertWasCalled(p => p.AddTrack(@".\This Developers Life\997.wma"));
			Playlist.AssertWasCalled(p => p.AddTrack(@".\This Developers Life\998.wma"));
			Playlist.AssertWasCalled(p => p.AddTrack(@".\This Developers Life\999.wma"));
		}

        [Test]
        public void ItShouldCopyThePlaylistToTheCorrectLocation()
        {
            FileUtilities.AssertWasCalled(utilities => utilities.FileCopy(@"c:\file.tmp", "MyPodcasts.wpl", true));
        }
    }
}