using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using PodcastUtilities.Common.IO;
using Rhino.Mocks;

namespace PodcastUtilities.Common.Tests.FileCopierTests
{
	public class WhenDestinationDriveIsAlreadyFull
		: SpecTestBase
	{
		private FileCopier FileCopier { get; set; }

		private IDriveInfo DestinationDriveInfo { get; set; }
		private IDriveInfoProvider DriveInfoProvider { get; set; }
		private IFileUtilities FileUtilities { get; set; }

		private List<SyncItem> SourceFiles { get; set; }

		private List<StatusUpdateEventArgs> StatusUpdates { get; set; }

		protected override void GivenThat()
		{
			base.GivenThat();

			DestinationDriveInfo = GenerateMock<IDriveInfo>();
			DestinationDriveInfo.Stub(i => i.AvailableFreeSpace).Return((1000 * 1024 * 1024) - 1);

			DriveInfoProvider = GenerateMock<IDriveInfoProvider>();
			DriveInfoProvider.Stub(p => p.GetDriveInfo(@"d:\Dest"))
				.Return(DestinationDriveInfo);

			FileUtilities = GenerateMock<IFileUtilities>();

			SourceFiles = new List<SyncItem>
			              	{
			              		new SyncItem {DestinationPath = "A", Source = new FileInfo(@"c:\Source\A")},
								new SyncItem {DestinationPath = "B", Source = new FileInfo(@"c:\Source\B")}
			              	};

			StatusUpdates = new List<StatusUpdateEventArgs>();

			FileCopier = new FileCopier(DriveInfoProvider, FileUtilities);
			FileCopier.StatusUpdate += (sender, e) => StatusUpdates.Add(e);
		}

		protected override void When()
		{
			FileCopier.CopyFilesToTarget(SourceFiles, @"c:\Source", @"d:\Dest", 1000, false);
		}

		[Test]
		public void ItShouldNotCopyAnyFiles()
		{
			FileUtilities.AssertWasNotCalled(
				f => f.FileCopy(null, null),
				o => o.IgnoreArguments());
		}

		[Test]
		public void ItShouldReportDriveFullStatusUpdates()
		{
			Assert.AreEqual(3, StatusUpdates.Count);

			Assert.AreEqual(StatusUpdateEventArgs.Level.Status, StatusUpdates[1].MessageLevel);
			Assert.AreEqual("Destination drive is full leaving 1,000 MB free", StatusUpdates[1].Message);
			Assert.AreEqual(StatusUpdateEventArgs.Level.Status, StatusUpdates[2].MessageLevel);
			Assert.AreEqual("Free Space on drive  is 1,023,999 KB, 999 MB, 0.98 GB", StatusUpdates[2].Message);
		}
	}
}
