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