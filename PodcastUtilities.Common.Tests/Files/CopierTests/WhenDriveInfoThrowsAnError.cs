using NUnit.Framework;
using Rhino.Mocks;

namespace PodcastUtilities.Common.Tests.Files.CopierTests
{
	public class WhenDriveInfoThrowsAnError
		: WhenTestingCopier
	{
		protected override void GivenThat()
		{
			base.GivenThat();

			DestinationePath = @"e:\error";
			FileUtilities.Stub(u => u.FileExists(@"e:\error\A"))
				.Return(false);
			FileUtilities.Stub(u => u.FileExists(@"e:\error\B"))
				.Return(false);
		}

		[Test]
		public void ItShouldReportErrorStatusUpdateOnlyOnce()
		{
			Assert.AreEqual(3, StatusUpdates.Count);		// one for each file and only one for the error

			Assert.AreEqual(StatusUpdateLevel.Warning, StatusUpdates[1].MessageLevel);
			Assert.IsTrue(StatusUpdates[1].Message.Contains("Object must be a root directory"));
		}

		[Test]
		public void ItShouldTryToCopyAllTheFiles()
		{
			FileUtilities.AssertWasCalled(u => u.FileCopy(@"c:\Source\A", @"e:\error\A"));
			FileUtilities.AssertWasCalled(u => u.FileCopy(@"c:\Source\B", @"e:\error\B"));
		}
	}
}