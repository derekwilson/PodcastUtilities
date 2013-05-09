using NUnit.Framework;
using Rhino.Mocks;

namespace PodcastUtilities.Common.Tests.Files.CopierTests
{
	public class WhenDestinationFileAlreadyExists
		: WhenTestingCopier
	{
		protected override void GivenThat()
		{
			base.GivenThat();

			DestinationDriveInfo.Stub(i => i.AvailableFreeSpace).Return(1001 * 1024 * 1024);

			FileUtilities.Stub(u => u.FileExists(@"d:\Dest\A"))
				.Return(true);

			FileUtilities.Stub(u => u.FileExists(@"d:\Dest\B"))
				.Return(false);
		}

		[Test]
		public void ItShouldNotCopyTheFileThatExists()
		{
			FileUtilities.AssertWasNotCalled(u => u.FileCopy(@"c:\Source\A", @"d:\Dest\A"));
		}

		[Test]
		public void ItShouldCopyTheOtherFiles()
		{
			FileUtilities.AssertWasCalled(u => u.FileCopy(@"c:\Source\B", @"d:\Dest\B"));
		}

		[Test]
		public void ItShouldOnlySendStatusUpdatesForFilesThatWereCopied()
		{
			Assert.AreEqual(1, StatusUpdates.Count);

			Assert.AreEqual(StatusUpdateLevel.Status, StatusUpdates[0].MessageLevel);
			Assert.AreEqual(@"Copying to: d:\Dest\B", StatusUpdates[0].Message);
		}

	}
}