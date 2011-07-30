using System.Collections.Generic;
using System.IO;
using PodcastUtilities.Common.Platform;
using Rhino.Mocks;

namespace PodcastUtilities.Common.Tests.FileCopierTests
{
	public abstract class WhenTestingFileCopier
		: WhenTestingBehaviour
	{
		protected FileCopier FileCopier { get; set; }
		
		protected IDriveInfo DestinationDriveInfo { get; set; }
		protected IDriveInfoProvider DriveInfoProvider { get; set; }
		protected IFileUtilities FileUtilities { get; set; }

		protected List<FileSyncItem> SourceFiles { get; set; }
		protected List<StatusUpdateEventArgs> StatusUpdates { get; set; }

		protected override void GivenThat()
		{
			base.GivenThat();

			DestinationDriveInfo = GenerateMock<IDriveInfo>();
			DestinationDriveInfo.Stub(i => i.Name).Return("D");

			DriveInfoProvider = GenerateMock<IDriveInfoProvider>();
			DriveInfoProvider.Stub(p => p.GetDriveInfo(@"d:\Dest"))
				.Return(DestinationDriveInfo);

			FileUtilities = GenerateMock<IFileUtilities>();

			SourceFiles = new List<FileSyncItem>
			              	{
			              		new FileSyncItem {Source = GenerateMock<IFileInfo>()},
			              		new FileSyncItem {Source = GenerateMock<IFileInfo>()}
			              	};
			SourceFiles[0].Source.Stub(s => s.FullName).Return(@"c:\Source\A");
			SourceFiles[1].Source.Stub(s => s.FullName).Return(@"c:\Source\B");

			StatusUpdates = new List<StatusUpdateEventArgs>();

			FileCopier = new FileCopier(DriveInfoProvider, FileUtilities);
			FileCopier.StatusUpdate += (sender, e) => StatusUpdates.Add(e);
		}

		protected override void When()
		{
			FileCopier.CopyFilesToTarget(SourceFiles, @"c:\Source", @"d:\Dest", 1000, false);
		}
	}
}