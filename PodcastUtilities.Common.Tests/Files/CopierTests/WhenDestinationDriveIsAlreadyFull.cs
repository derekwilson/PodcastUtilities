using NUnit.Framework;
using Rhino.Mocks;

namespace PodcastUtilities.Common.Tests.Files.CopierTests
{
	public class WhenDestinationDriveIsAlreadyFull
		: WhenTestingCopier
	{
		protected override void GivenThat()
		{
			base.GivenThat();

			DestinationDriveInfo.Stub(i => i.AvailableFreeSpace).Return(999 * 1024 * 1024);
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
			Assert.AreEqual("Free Space on drive D is 1,022,976 KB, 999 MB, 0.98 GB", StatusUpdates[2].Message);
		}
	}
}
