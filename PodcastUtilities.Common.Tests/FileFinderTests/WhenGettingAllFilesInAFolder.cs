using NUnit.Framework;
using Rhino.Mocks;
using R = Rhino.Mocks.Constraints;

namespace PodcastUtilities.Common.Tests.FileFinderTests
{
	public class WhenGettingAllFilesInAFolder : WhenTestingFileFinder
	{
		protected override void When()
		{
			FoundFiles = FileFinder.GetFiles(@"d:\podcasts", "*.wma");
		}

		[Test]
		public void ItShouldGetTheDirectoryInfoFromTheProvider()
		{
			DirectoryInfoProvider.AssertWasCalled(d => d.GetDirectoryInfo(@"d:\podcasts"));
		}

		[Test]
		public void ItShouldGetTheFilesFromTheDirectoryInfo()
		{
			DirectoryInfo.AssertWasCalled(d => d.GetFiles("*.wma"));
		}

		[Test]
		public void ItShouldReturnTheCorrectFiles()
		{
			Assert.AreEqual(3, FoundFiles.Count);
			Assert.AreEqual(FilesInDirectory[0], FoundFiles[0]);
			Assert.AreEqual(FilesInDirectory[1], FoundFiles[1]);
			Assert.AreEqual(FilesInDirectory[2], FoundFiles[2]);
		}
	}
}