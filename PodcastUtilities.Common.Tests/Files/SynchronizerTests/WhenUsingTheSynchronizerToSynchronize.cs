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
using PodcastUtilities.Common.Files;
using PodcastUtilities.Common.Platform;
using Rhino.Mocks;
using Is = Rhino.Mocks.Constraints.Is;

namespace PodcastUtilities.Common.Tests.Files.SynchronizerTests
{
	public class WhenUsingTheSynchronizerToSynchronize
		: WhenTestingTheSynchronizer
	{
		protected List<IFileInfo> PodcastFiles1 { get; set; }
		protected List<IFileInfo> PodcastFiles2 { get; set; }

		protected List<FileSyncItem> FilesToCopy { get; set; }

		protected override void GivenThat()
		{
			base.GivenThat();

			var podcast1 = new PodcastInfo(ControlFile)
			               	{
			               		Folder = "pod1"
			               	};
            podcast1.Pattern.Value = "*.mp3";
		    podcast1.MaximumNumberOfFiles.Value = 2;
		    podcast1.AscendingSort.Value = true;
		    podcast1.DeleteEmptyFolder.Value = true;
		    podcast1.SortField.Value = PodcastFileSortField.FileName;

			var podcast2 = new PodcastInfo(ControlFile)
			               	{
			               		Folder = "AnotherPodcast"
			               	};
            podcast2.Pattern.Value = "*.wma";
            podcast2.MaximumNumberOfFiles.Value = 3;
            podcast2.AscendingSort.Value = false;
            podcast2.DeleteEmptyFolder.Value = false;
            podcast2.SortField.Value = PodcastFileSortField.CreationTime;

			PodcastFiles1 = new List<IFileInfo> {GenerateMock<IFileInfo>(), GenerateMock<IFileInfo>()};
			PodcastFiles2 = new List<IFileInfo> {GenerateMock<IFileInfo>(), GenerateMock<IFileInfo>(), GenerateMock<IFileInfo>()};

			ControlFile.Stub(c => c.GetPodcasts())
				.Return(new List<IPodcastInfo> {podcast1, podcast2});

			ControlFile.Stub(c => c.GetSourceRoot())
				.Return(@"c:\media\blah");
			ControlFile.Stub(c => c.GetDestinationRoot())
				.Return(@"k:\podcasts");
			ControlFile.Stub(c => c.GetFreeSpaceToLeaveOnDestination())
				.Return(500);

            FileFinder.Stub(f => f.GetFiles(@"c:\media\blah\pod1", "*.mp3", 2, PodcastFileSortField.FileName, true))
				.Return(PodcastFiles1);
            FileFinder.Stub(f => f.GetFiles(@"c:\media\blah\AnotherPodcast", "*.wma", 3, PodcastFileSortField.CreationTime, false))
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
            FileFinder.AssertWasCalled(f => f.GetFiles(@"c:\media\blah\pod1", "*.mp3", 2, PodcastFileSortField.FileName, true));

            FileFinder.AssertWasCalled(f => f.GetFiles(@"c:\media\blah\AnotherPodcast", "*.wma", 3, PodcastFileSortField.CreationTime, false));
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
				o => o.Constraints(Is.NotNull(), Is.Equal(@"c:\media\blah"), Is.Equal(@"k:\podcasts"), Is.Equal(500L), Is.Equal(false)));
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

	    [Test]
	    public void ItShouldRemoveUnwantedFoldersFromEachPodcast()
	    {
            FolderRemover.AssertWasCalled(r => r.RemoveFolderIfEmpty(@"k:\podcasts\pod1", false));
            FolderRemover.AssertWasNotCalled(r => r.RemoveFolderIfEmpty(@"k:\podcasts\AnotherPodcast", false));
        }
	}
}
