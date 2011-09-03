using NUnit.Framework;
using Rhino.Mocks;

namespace PodcastUtilities.Common.Tests.Files.FinderTests
{
	public class WhenGettingUnlimitedSortedFilesInAFolder : WhenTestingTheFinder
	{
		protected override void When()
		{
			FoundFiles = FileFinder.GetFiles(@"c:\blah", "*.mp3", -1, "name", true);
		}

		[Test]
		public void ItShouldGetTheDirectoryInfoFromTheProvider()
		{
			DirectoryInfoProvider.AssertWasCalled(d => d.GetDirectoryInfo(@"c:\blah"));
		}

		[Test]
		public void ItShouldGetTheFilesFromTheDirectoryInfo()
		{
			DirectoryInfo.AssertWasCalled(d => d.GetFiles("*.mp3"));
		}

		[Test]
		public void ItShouldSortTheFiles()
		{
			FileSorter.AssertWasCalled(
				s => s.Sort(null, "name", true),
				o => o.Constraints(Rhino.Mocks.Constraints.Property.Value("Count", 3), Rhino.Mocks.Constraints.Is.Equal("name"), Rhino.Mocks.Constraints.Is.Equal(true)));
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