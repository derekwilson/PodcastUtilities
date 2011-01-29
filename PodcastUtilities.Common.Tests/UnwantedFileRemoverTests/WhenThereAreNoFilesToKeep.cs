using NUnit.Framework;
using Rhino.Mocks;

namespace PodcastUtilities.Common.Tests.UnwantedFileRemoverTests
{
	public class WhenThereAreNoFilesToKeep : WhenTestingUnwantedFileRemover
	{
		protected override void GivenThat()
		{
			base.GivenThat();

			DirectoryInfo.Stub(d => d.Exists)
				.Return(true);
		}

		protected override void When()
		{
			FileRemover.RemoveUnwantedFiles(FilesToKeep, @"c:\x\y\z", "*.mp3", false);
		}

		[Test]
		public void ItShouldGetTheDirectoryFromTheProvider()
		{
			DirectoryInfoProvider.AssertWasCalled(d => d.GetDirectoryInfo(@"c:\x\y\z"));
		}

		[Test]
		public void ItShouldGetFilesInTheDirectory()
		{
			DirectoryInfo.AssertWasCalled(d => d.GetFiles("*.mp3"));
		}

		[Test]
		public void ItShouldDeleteAllTheFiles()
		{
			FileUtilities.AssertWasCalled(u => u.FileDelete(@"c:\a\b\c\1.mp3"));
			FileUtilities.AssertWasCalled(u => u.FileDelete(@"c:\a\b\c\2.mp3"));
			FileUtilities.AssertWasCalled(u => u.FileDelete(@"c:\a\b\c\3.mp3"));
		}

		[Test]
		public void ItShouldSendStatusUpdatesForEachDeletedFile()
		{
			Assert.AreEqual(3, StatusUpdates.Count);

			Assert.That(StatusUpdates[0].Message.Contains(FilesInDirectory[0].FullName));
			Assert.That(StatusUpdates[1].Message.Contains(FilesInDirectory[1].FullName));
			Assert.That(StatusUpdates[2].Message.Contains(FilesInDirectory[2].FullName));
		}
	}
}