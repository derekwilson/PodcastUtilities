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
using Moq;
using NUnit.Framework;
using PodcastUtilities.Common.Platform;

namespace PodcastUtilities.Common.Multiplatform.Tests.Files.UnwantedFileRemoverTests
{
    public class WhenThereAreFilesToKeep : WhenTestingUnwantedFileRemover
    {
        protected Mock<IFileInfo> _keepfile1;
        protected Mock<IFileInfo> _keepfile2;

        protected override void GivenThat()
        {
            base.GivenThat();

            DirectoryInfo.Setup(d => d.Exists)
                .Returns(true);

            _keepfile1 = GenerateMock<IFileInfo>();
            _keepfile2 = GenerateMock<IFileInfo>();

            FilesToKeep.Add(_keepfile1.Object);
            FilesToKeep.Add(_keepfile2.Object);

            _keepfile1.Setup(f => f.Name)
                .Returns(@"1.mp3");
            _keepfile2.Setup(f => f.Name)
                .Returns(@"3.mp3");
        }

        protected override void When()
        {
            FileRemover.RemoveUnwantedFiles(FilesToKeep, @"c:\x\y\z", "*.mp3", false);
        }

        [Test]
        public void ItShouldGetTheDirectoryFromTheProvider()
        {
            DirectoryInfoProvider.Verify(info => info.GetDirectoryInfo(@"c:\x\y\z"), Times.Once());
        }

        [Test]
        public void ItShouldGetFilesInTheDirectory()
        {
            DirectoryInfo.Verify(info => info.GetFiles("*.mp3"), Times.Once());
        }

        [Test]
        public void ItShouldDeleteTheFilesNotToBeKept()
        {
            FileUtilities.Verify(utils => utils.FileDelete(@"c:\a\b\c\2.mp3"), Times.Once());
        }

        [Test]
        public void ItShouldNotDeleteTheFilesToBeKept()
        {
            FileUtilities.Verify(utils => utils.FileDelete(@"c:\a\b\c\1.mp3"), Times.Never());
            FileUtilities.Verify(utils => utils.FileDelete(@"c:\a\b\c\3.mp3"), Times.Never());
        }

        [Test]
        public void ItShouldSendStatusUpdatesForEachDeletedFile()
        {
            Assert.AreEqual(1, StatusUpdates.Count);

            Assert.That(StatusUpdates[0].Message.Contains(FilesInDirectory[1].FullName));
        }
    }
}