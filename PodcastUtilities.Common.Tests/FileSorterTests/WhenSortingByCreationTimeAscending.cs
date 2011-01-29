using NUnit.Framework;

namespace PodcastUtilities.Common.Tests.FileSorterTests
{
	public class WhenSortingByCreationTimeAscending : WhenTestingFileSorter
	{
		protected override void When()
		{
			FileSorter.Sort(SortedFiles, "creationtime", true);
		}

		[Test]
		public void ItShouldSortTheFilesCorrectly()
		{
			Assert.AreEqual(OriginalFiles[1], SortedFiles[0]);
			Assert.AreEqual(OriginalFiles[2], SortedFiles[1]);
			Assert.AreEqual(OriginalFiles[0], SortedFiles[2]);
		}
	}
}