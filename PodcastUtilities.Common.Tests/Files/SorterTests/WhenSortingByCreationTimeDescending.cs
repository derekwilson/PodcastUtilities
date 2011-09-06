using NUnit.Framework;

namespace PodcastUtilities.Common.Tests.Files.SorterTests
{
	public class WhenSortingByCreationTimeDescending : WhenTestingTheSorter
	{
		protected override void When()
		{
            SortedFiles = FileSorter.Sort(OriginalFiles, "creationtime", false);
		}

		[Test]
		public void ItShouldSortTheFilesCorrectly()
		{
			Assert.AreEqual(OriginalFiles[0], SortedFiles[0]);
			Assert.AreEqual(OriginalFiles[2], SortedFiles[1]);
			Assert.AreEqual(OriginalFiles[1], SortedFiles[2]);
		}
	}
}