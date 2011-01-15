using System.Collections.Generic;
using System.IO;
using PodcastUtilities.Common.IO;
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

		protected List<SyncItem> SourceFiles { get; set; }
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

			SourceFiles = new List<SyncItem>
			              	{
			              		new SyncItem {Source = new FileInfo(@"c:\Source\A")},
			              		new SyncItem {Source = new FileInfo(@"c:\Source\B")}
			              	};

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