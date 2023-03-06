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

namespace PodcastUtilities.Common.Multiplatform.Tests.Files.CopierTests
{
    public abstract class WhenTestingCopier
        : WhenTestingBehaviour
    {
        protected Copier FileCopier { get; set; }

        protected Mock<IDriveInfo> DestinationDriveInfo { get; set; }
        protected Mock<IDriveInfoProvider> DriveInfoProvider { get; set; }
        protected Mock<IFileUtilities> FileUtilities { get; set; }
        protected Mock<IPathUtilities> PathUtilities { get; set; }
        protected Mock<IFileInfo> FileInfo1 { get; set; }
        protected Mock<IFileInfo> FileInfo2 { get; set; }

        protected List<FileSyncItem> SourceFiles { get; set; }
        protected List<StatusUpdateEventArgs> StatusUpdates { get; set; }

        protected string SourcePath { get; set; }
        protected string DestinationePath { get; set; }

        protected override void GivenThat()
        {
            base.GivenThat();

            SourcePath = @"c:\Source";
            DestinationePath = @"d:\Dest";

            DestinationDriveInfo = GenerateMock<IDriveInfo>();
            DestinationDriveInfo.Setup(i => i.Name).Returns("D");

            DriveInfoProvider = GenerateMock<IDriveInfoProvider>();
            DriveInfoProvider.Setup(p => p.GetDriveInfoForPath(@"d:\Dest"))
                .Returns(DestinationDriveInfo.Object);
            DriveInfoProvider.Setup(p => p.GetDriveInfoForPath(@"e:\error"))
                .Throws(new System.ArgumentException(@"Object must be a root directory ('C:\') or a drive letter ('C')."));

            FileUtilities = GenerateMock<IFileUtilities>();
            PathUtilities = GenerateMock<IPathUtilities>();

            PathUtilities.Setup(pathUtilities => pathUtilities.GetFullPath(It.IsAny<string>()))
                .Returns<string>(param => param);

            FileInfo1 = GenerateMock<IFileInfo>();
            FileInfo2 = GenerateMock<IFileInfo>();

            SourceFiles = new List<FileSyncItem>
                              {
                                  new FileSyncItem {Source = FileInfo1.Object},
                                  new FileSyncItem {Source = FileInfo2.Object}
                              };
            FileInfo1.Setup(s => s.FullName).Returns(@"c:\Source\A");
            FileInfo2.Setup(s => s.FullName).Returns(@"c:\Source\B");

            StatusUpdates = new List<StatusUpdateEventArgs>();

            FileCopier = new Copier(DriveInfoProvider.Object, FileUtilities.Object, PathUtilities.Object);
            FileCopier.StatusUpdate += (sender, e) => StatusUpdates.Add(e);
        }

        protected override void When()
        {
            FileCopier.CopyFilesToTarget(SourceFiles, SourcePath, DestinationePath, 1000, false);
        }
    }
}