using NUnit.Framework;
using PodcastUtilities.Common.IO;
using Rhino.Mocks;

namespace PodcastUtilities.Common.Tests.UnwantedFileRemoverTests
{
	public class WhenThereAreFilesToKeep : WhenTestingUnwantedFileRemover
	{
		protected override void GivenThat()
		{
			base.GivenThat();

			DirectoryInfo.Stub(d => d.Exists)
				.Return(true);

			FilesToKeep.Add(GenerateMock<IFileInfo>());
			FilesToKeep.Add(GenerateMock<IFileInfo>());

			FilesToKeep[0].Stub(f => f.Name)
				.Return(@"1.mp3");
			FilesToKeep[1].Stub(f => f.Name)
				.Return(@"3.mp3");
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
		public void ItShouldDeleteTheFilesNotToBeKept()
		{
			FileUtilities.AssertWasCalled(u => u.FileDelete(@"c:\a\b\c\2.mp3"));
		}

		[Test]
		public void ItShouldNotDeleteTheFilesToBeKept()
		{
			FileUtilities.AssertWasNotCalled(u => u.FileDelete(@"c:\a\b\c\1.mp3"));
			FileUtilities.AssertWasNotCalled(u => u.FileDelete(@"c:\a\b\c\3.mp3"));
		}

		[Test]
		public void ItShouldSendStatusUpdatesForEachDeletedFile()
		{
			Assert.AreEqual(1, StatusUpdates.Count);

			Assert.That(StatusUpdates[0].Message.Contains(FilesInDirectory[1].FullName));
		}
	}
}