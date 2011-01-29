using NUnit.Framework;
using Rhino.Mocks;

namespace PodcastUtilities.Common.Tests.UnwantedFileRemoverTests
{
	public class WhenDestinationDirectoryDoesNotExist : WhenTestingUnwantedFileRemover
	{
		protected override void GivenThat()
		{
			base.GivenThat();

			DirectoryInfo.Stub(d => d.Exists)
				.Return(false);
		}

		protected override void When()
		{
			FileRemover.RemoveUnwantedFiles(FilesToKeep, @"c:\blah", "*.wma", false);
		}

		[Test]
		public void ItShouldGetTheDirectoryFromTheProvider()
		{
			DirectoryInfoProvider.AssertWasCalled(d => d.GetDirectoryInfo(@"c:\blah"));
		}

		[Test]
		public void ItShouldNotTryToGetFilesInTheDirectory()
		{
			DirectoryInfo.AssertWasNotCalled(
				d => d.GetFiles(null),
				o => o.IgnoreArguments());
		}

		[Test]
		public void ItShouldNotTryToDeleteAnyFiles()
		{
			FileUtilities.AssertWasNotCalled(
				u => u.FileDelete(null),
				o => o.IgnoreArguments());
		}

		[Test]
		public void ItShouldNotSendAnyStatusUpdates()
		{
			Assert.AreEqual(0, StatusUpdates.Count);
		}
	}
}