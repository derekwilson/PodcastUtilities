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
using PodcastUtilities.Common.Configuration;
using PodcastUtilities.Common.Files;
using PodcastUtilities.Common.Platform;

namespace PodcastUtilities.Common.Multiplatform.Tests.Files.FinderTests
{
    public abstract class WhenTestingTheFinder
        : WhenTestingBehaviour
    {
        protected Finder FileFinder { get; set; }

        protected Mock<ISorter> FileSorter { get; set; }
        protected Mock<IDirectoryInfoProvider> DirectoryInfoProvider { get; set; }
        protected Mock<IDirectoryInfo> DirectoryInfo { get; set; }

        protected IFileInfo[] FilesInDirectory { get; set; }
        protected Mock<IFileInfo> _file1;
        protected Mock<IFileInfo> _file2;
        protected Mock<IFileInfo> _file3;

        protected IList<IFileInfo> FoundFiles { get; set; }


        protected override void GivenThat()
        {
            base.GivenThat();

            FileSorter = GenerateMock<ISorter>();
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

            DirectoryInfo.Setup(d => d.GetFiles(It.IsAny<string>()))
                .Returns(FilesInDirectory);

            DirectoryInfoProvider.Setup(d => d.GetDirectoryInfo(It.IsAny<string>()))
                .Returns(DirectoryInfo.Object);

            FileSorter.Setup(finder => finder.Sort(FilesInDirectory, PodcastFileSortField.FileName, true)).Returns(FilesInDirectory);

            FileFinder = new Finder(FileSorter.Object, DirectoryInfoProvider.Object);
        }
    }
}
