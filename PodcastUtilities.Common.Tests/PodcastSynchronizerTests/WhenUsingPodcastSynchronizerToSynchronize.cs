using System.Collections.Generic;
using NUnit.Framework;
using PodcastUtilities.Common.IO;
using Rhino.Mocks;
using R = Rhino.Mocks.Constraints;

namespace PodcastUtilities.Common.Tests.PodcastSynchronizerTests
{
	public class WhenUsingPodcastSynchronizerToSynchronize
		: WhenTestingPodcastSynchronizer
	{
		protected List<IFileInfo> PodcastFiles1 { get; set; }
		protected List<IFileInfo> PodcastFiles2 { get; set; }

		protected List<FileSyncItem> FilesToCopy { get; set; }

		protected override void GivenThat()
		{
			base.GivenThat();

			var podcast1 = new PodcastInfo
			               	{
			               		Folder = "pod1",
			               		Pattern = "*.mp3",
			               		MaximumNumberOfFiles = 2,
			               		SortField = "name",
			               		AscendingSort = true
			               	};
			var podcast2 = new PodcastInfo
			               	{
			               		Folder = "AnotherPodcast",
			               		Pattern = "*.wma",
			               		MaximumNumberOfFiles = 3,
			               		SortField = "creationtime",
			               		AscendingSort = false
			               	};

			PodcastFiles1 = new List<IFileInfo> {GenerateMock<IFileInfo>(), GenerateMock<IFileInfo>()};
			PodcastFiles2 = new List<IFileInfo> {GenerateMock<IFileInfo>(), GenerateMock<IFileInfo>(), GenerateMock<IFileInfo>()};

			ControlFile.Stub(c => c.Podcasts)
				.Return(new List<PodcastInfo> {podcast1, podcast2});

			ControlFile.Stub(c => c.SourceRoot)
				.Return(@"c:\media\blah");
			ControlFile.Stub(c => c.DestinationRoot)
				.Return(@"k:\podcasts");
			ControlFile.Stub(c => c.FreeSpaceToLeaveOnDestination)
				.Return(500);

			FileFinder.Stub(f => f.GetFiles(@"c:\media\blah\pod1", "*.mp3", 2, "name", true))
				.Return(PodcastFiles1);
			FileFinder.Stub(f => f.GetFiles(@"c:\media\blah\AnotherPodcast", "*.wma", 3, "creationtime", false))
				.Return(PodcastFiles2);

			FileCopier.Stub(c => c.CopyFilesToTarget(null, null, null, 0, false))
				.IgnoreArguments()
				.WhenCalled(invocation => FilesToCopy = (List<FileSyncItem>) invocation.Arguments[0]);
		}

		protected override void When()
		{
			PodcastSynchronizer.Synchronize(ControlFile, false);
		}

		[Test]
		public void ItShouldFindTheSourceFilesForEachPodcast()
		{
			FileFinder.AssertWasCalled(f => f.GetFiles(@"c:\media\blah\pod1", "*.mp3", 2, "name", true));

			FileFinder.AssertWasCalled(f => f.GetFiles(@"c:\media\blah\AnotherPodcast", "*.wma", 3, "creationtime", false));
		}

		[Test]
		public void ItShouldRemoveUnwantedFilesFromEachPodcastDestination()
		{
			FileRemover.AssertWasCalled(r => r.RemoveUnwantedFiles(PodcastFiles1, @"k:\podcasts\pod1", "*.mp3", false));

			FileRemover.AssertWasCalled(r => r.RemoveUnwantedFiles(PodcastFiles2, @"k:\podcasts\AnotherPodcast", "*.wma", false));
		}

		[Test]
		public void ItShouldUseTheFileCopierToCopyTheFiles()
		{
			FileCopier.AssertWasCalled(
				c => c.CopyFilesToTarget(null, null, null, 0, false),
				o => o.Constraints(R.Is.NotNull(), R.Is.Equal(@"c:\media\blah"), R.Is.Equal(@"k:\podcasts"), R.Is.Equal(500L), R.Is.Equal(false)));
		}

		[Test]
		public void ItShouldCopyAllTheFiles()
		{
			Assert.AreEqual(5, FilesToCopy.Count);

			Assert.AreEqual(PodcastFiles1[0], FilesToCopy[0].Source);
			Assert.AreEqual(PodcastFiles1[1], FilesToCopy[1].Source);
			Assert.AreEqual(PodcastFiles2[0], FilesToCopy[2].Source);
			Assert.AreEqual(PodcastFiles2[1], FilesToCopy[3].Source);
			Assert.AreEqual(PodcastFiles2[2], FilesToCopy[4].Source);
		}
	}
}
