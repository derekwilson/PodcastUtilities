﻿using NUnit.Framework;

namespace PodcastUtilities.Common.Tests.Files.SorterTests
{
	public class WhenSortingByNameAscending : WhenTestingTheSorter
	{
		protected override void When()
		{
			FileSorter.Sort(SortedFiles, "name", true);
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