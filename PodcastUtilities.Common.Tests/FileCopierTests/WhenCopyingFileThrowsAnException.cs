using System.IO;
using NUnit.Framework;
using Rhino.Mocks;

namespace PodcastUtilities.Common.Tests.FileCopierTests
{
	public class WhenCopyingFileThrowsAnException
		: WhenTestingFileCopier
	{
		protected override void GivenThat()
		{
			base.GivenThat();

			DestinationDriveInfo.Stub(i => i.AvailableFreeSpace).Return(1001 * 1024 * 1024);

			FileUtilities.Stub(u => u.FileCopy(null, null))
				.IgnoreArguments()
				.Throw(new FileNotFoundException("blah"));
		}

		[Test]
		public void ItShouldReportErrorStatusUpdate()
		{
			Assert.AreEqual(2, StatusUpdates.Count);

			Assert.AreEqual(StatusUpdateEventArgs.Level.Error, StatusUpdates[1].MessageLevel);
			Assert.AreEqual("Error writing file: blah", StatusUpdates[1].Message);
		}

	}
}