using NUnit.Framework;
using PodcastUtilities.Common.Configuration;
using Rhino.Mocks;

namespace PodcastUtilities.Common.Tests.Files.FinderTests
{
	public class WhenGettingLessThanAllFilesInAFolder : WhenTestingTheFinder
	{
		protected override void When()
		{
            FoundFiles = FileFinder.GetFiles(@"c:\blah", "*.mp3", 2, PodcastFileSortField.FileName, true);
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
                s => s.Sort(null, PodcastFileSortField.FileName, true),
                o => o.Constraints(Rhino.Mocks.Constraints.Property.Value("Count", 3), Rhino.Mocks.Constraints.Is.Equal(PodcastFileSortField.FileName), Rhino.Mocks.Constraints.Is.Equal(true)));
		}

		[Test]
		public void ItShouldReturnTheCorrectSubsetOfFiles()
		{
			Assert.AreEqual(2, FoundFiles.Count);
			Assert.AreEqual(FilesInDirectory[0], FoundFiles[0]);
			Assert.AreEqual(FilesInDirectory[1], FoundFiles[1]);
		}
	}
}