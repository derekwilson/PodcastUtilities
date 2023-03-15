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
using System.Collections.ObjectModel;
using Moq;
using NUnit.Framework;
using PodcastUtilities.Common.Configuration;
using PodcastUtilities.Common.Files;
using PodcastUtilities.Common.Platform;

namespace PodcastUtilities.Common.Multiplatform.Tests.Files.SynchronizerTests
{
    public class WhenUsingTheSynchronizerToSynchronize
        : WhenTestingTheSynchronizer
    {
        protected List<IFileInfo> PodcastFiles1 { get; set; }
        protected List<IFileInfo> PodcastFiles2 { get; set; }

        protected List<FileSyncItem> FilesToCopy { get; set; }

        protected Mock<IFileInfo> _file1;
        protected Mock<IFileInfo> _file2;
        protected Mock<IFileInfo> _file3;
        protected Mock<IFileInfo> _file4;
        protected Mock<IFileInfo> _file5;

        protected override void GivenThat()
        {
            base.GivenThat();

            var podcast1 = new PodcastInfo(ControlFile.Object)
            {
                Folder = "pod1"
            };
            podcast1.Pattern.Value = "*.mp3";
            podcast1.MaximumNumberOfFiles.Value = 2;
            podcast1.AscendingSort.Value = true;
            podcast1.DeleteEmptyFolder.Value = true;
            podcast1.SortField.Value = PodcastFileSortField.FileName;

            var podcast2 = new PodcastInfo(ControlFile.Object)
            {
                Folder = "AnotherPodcast"
            };
            podcast2.Pattern.Value = "*.wma";
            podcast2.MaximumNumberOfFiles.Value = 3;
            podcast2.AscendingSort.Value = false;
            podcast2.DeleteEmptyFolder.Value = false;
            podcast2.SortField.Value = PodcastFileSortField.CreationTime;

            _file1 = GenerateMock<IFileInfo>();
            _file2 = GenerateMock<IFileInfo>();
            _file3 = GenerateMock<IFileInfo>();
            _file4 = GenerateMock<IFileInfo>();
            _file5 = GenerateMock<IFileInfo>();

            PodcastFiles1 = new List<IFileInfo> { _file1.Object, _file2.Object };
            PodcastFiles2 = new List<IFileInfo> { _file3.Object, _file4.Object, _file5.Object };

            ControlFile.Setup(c => c.GetPodcasts())
                .Returns(new List<IPodcastInfo> { podcast1, podcast2 });

            ControlFile.Setup(c => c.GetSourceRoot())
                .Returns(@"c:\media\blah");
            ControlFile.Setup(c => c.GetDestinationRoot())
                .Returns(@"k:\podcasts");
            ControlFile.Setup(c => c.GetFreeSpaceToLeaveOnDestination())
                .Returns(500);

            FileFinder.Setup(f => f.GetFiles(@"c:\media\blah\pod1", "*.mp3", 2, PodcastFileSortField.FileName, true))
                .Returns(PodcastFiles1);
            FileFinder.Setup(f => f.GetFiles(@"c:\media\blah\AnotherPodcast", "*.wma", 3, PodcastFileSortField.CreationTime, false))
                .Returns(PodcastFiles2);

            FileCopier.Setup(c => c.CopyFilesToTarget(It.IsAny<IEnumerable<FileSyncItem>>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<long>(), It.IsAny<bool>()))
                .Callback(new InvocationAction(i => FilesToCopy = i.Arguments[0] as List<FileSyncItem>));
        }

        protected override void When()
        {
            PodcastSynchronizer.Synchronize(ControlFile.Object, false);
        }

        [Test]
        public void ItShouldFindTheSourceFilesForEachPodcast()
        {
            FileFinder.Verify(f => f.GetFiles(@"c:\media\blah\pod1", "*.mp3", 2, PodcastFileSortField.FileName, true), Times.Once());

            FileFinder.Verify(f => f.GetFiles(@"c:\media\blah\AnotherPodcast", "*.wma", 3, PodcastFileSortField.CreationTime, false), Times.Once());
        }

        [Test]
        public void ItShouldRemoveUnwantedFilesFromEachPodcastDestination()
        {
            FileRemover.Verify(r => r.RemoveUnwantedFiles(PodcastFiles1, @"k:\podcasts\pod1", "*.mp3", false), Times.Once());

            FileRemover.Verify(r => r.RemoveUnwantedFiles(PodcastFiles2, @"k:\podcasts\AnotherPodcast", "*.wma", false), Times.Once());
        }

        [Test]
        public void ItShouldUseTheFileCopierToCopyTheFiles()
        {
            FileCopier.Verify(c => c.CopyFilesToTarget(It.IsAny<IEnumerable<FileSyncItem>>(), @"c:\media\blah", @"k:\podcasts", 500L, false), Times.Once());
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
            FolderRemover.Verify(r => r.RemoveFolderIfEmpty(@"k:\podcasts\pod1", false), Times.Once());
            FolderRemover.Verify(r => r.RemoveFolderIfEmpty(@"k:\podcasts\AnotherPodcast", false), Times.Never());
        }
    }
}
