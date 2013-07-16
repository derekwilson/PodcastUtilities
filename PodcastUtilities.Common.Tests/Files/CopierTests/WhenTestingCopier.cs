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
using PodcastUtilities.Common.Files;
using PodcastUtilities.Common.Platform;
using Rhino.Mocks;

namespace PodcastUtilities.Common.Tests.Files.CopierTests
{
	public abstract class WhenTestingCopier
		: WhenTestingBehaviour
	{
		protected Copier FileCopier { get; set; }
		
		protected IDriveInfo DestinationDriveInfo { get; set; }
		protected IDriveInfoProvider DriveInfoProvider { get; set; }
		protected IFileUtilities FileUtilities { get; set; }
		protected IPathUtilities PathUtilities { get; set; }

		protected List<FileSyncItem> SourceFiles { get; set; }
		protected List<StatusUpdateEventArgs> StatusUpdates { get; set; }

		protected override void GivenThat()
		{
			base.GivenThat();

			DestinationDriveInfo = GenerateMock<IDriveInfo>();
			DestinationDriveInfo.Stub(i => i.Name).Return("D");

			DriveInfoProvider = GenerateMock<IDriveInfoProvider>();
			DriveInfoProvider.Stub(p => p.GetDriveInfoForPath(@"d:\Dest"))
				.Return(DestinationDriveInfo);

			FileUtilities = GenerateMock<IFileUtilities>();
			PathUtilities = GenerateMock<IPathUtilities>();

		    PathUtilities.Stub(pathUtilities => pathUtilities.GetFullPath(null))
		        .IgnoreArguments()
		        .WhenCalled(invocation => invocation.ReturnValue = invocation.Arguments[0])
                .Return(null);

			SourceFiles = new List<FileSyncItem>
			              	{
			              		new FileSyncItem {Source = GenerateMock<IFileInfo>()},
			              		new FileSyncItem {Source = GenerateMock<IFileInfo>()}
			              	};
			SourceFiles[0].Source.Stub(s => s.FullName).Return(@"c:\Source\A");
			SourceFiles[1].Source.Stub(s => s.FullName).Return(@"c:\Source\B");

			StatusUpdates = new List<StatusUpdateEventArgs>();

			FileCopier = new Copier(DriveInfoProvider, FileUtilities, PathUtilities);
			FileCopier.StatusUpdate += (sender, e) => StatusUpdates.Add(e);
		}

		protected override void When()
		{
			FileCopier.CopyFilesToTarget(SourceFiles, @"c:\Source", @"d:\Dest", 1000, false);
		}
	}
}