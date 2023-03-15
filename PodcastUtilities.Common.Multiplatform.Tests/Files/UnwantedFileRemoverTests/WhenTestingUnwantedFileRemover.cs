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
using Moq;
using PodcastUtilities.Common.Files;
using PodcastUtilities.Common.Platform;

namespace PodcastUtilities.Common.Multiplatform.Tests.Files.UnwantedFileRemoverTests
{
    public abstract class WhenTestingUnwantedFileRemover
        : WhenTestingBehaviour
    {
        protected UnwantedFileRemover FileRemover { get; set; }

        protected Mock<IFileUtilities> FileUtilities { get; set; }
        protected Mock<IDirectoryInfoProvider> DirectoryInfoProvider { get; set; }
        protected Mock<IDirectoryInfo> DirectoryInfo { get; set; }

        protected IFileInfo[] FilesInDirectory { get; set; }

        protected List<IFileInfo> FilesToKeep { get; set; }

        protected List<StatusUpdateEventArgs> StatusUpdates { get; set; }

        protected Mock<IFileInfo> _file1;
        protected Mock<IFileInfo> _file2;
        protected Mock<IFileInfo> _file3;

        protected override void GivenThat()
        {
            base.GivenThat();

            FileUtilities = GenerateMock<IFileUtilities>();
            DirectoryInfoProvider = GenerateMock<IDirectoryInfoProvider>();
            DirectoryInfo = GenerateMock<IDirectoryInfo>();

            _file1 = GenerateMock<IFileInfo>();
            _file2 = GenerateMock<IFileInfo>();
            _file3 = GenerateMock<IFileInfo>();

            FilesInDirectory = new IFileInfo[]
                                   {
                                       _file1.Object,
                                       _file2.Object,
                                       _file3.Object,
                                   };
            _file1.Setup(f => f.FullName).Returns(@"c:\a\b\c\1.mp3");
            _file1.Setup(f => f.Name).Returns(@"1.mp3");
            _file2.Setup(f => f.FullName).Returns(@"c:\a\b\c\2.mp3");
            _file2.Setup(f => f.Name).Returns(@"2.mp3");
            _file3.Setup(f => f.FullName).Returns(@"c:\a\b\c\3.mp3");
            _file3.Setup(f => f.Name).Returns(@"3.mp3");

            FilesToKeep = new List<IFileInfo>();

            DirectoryInfo.Setup(d => d.GetFiles(It.IsAny<string>()))
                .Returns(FilesInDirectory);

            DirectoryInfoProvider.Setup(d => d.GetDirectoryInfo(It.IsAny<string>()))
                .Returns(DirectoryInfo.Object);

            StatusUpdates = new List<StatusUpdateEventArgs>();

            FileRemover = new UnwantedFileRemover(DirectoryInfoProvider.Object, FileUtilities.Object);
            FileRemover.StatusUpdate += FileRemoverStatusUpdate;
        }

        void FileRemoverStatusUpdate(object sender, StatusUpdateEventArgs e)
        {
            StatusUpdates.Add(e);
        }
    }
}
