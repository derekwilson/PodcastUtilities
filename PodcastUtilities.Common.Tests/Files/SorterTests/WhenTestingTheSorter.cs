using System;
using System.Collections.Generic;
using PodcastUtilities.Common.Files;
using PodcastUtilities.Common.Platform;
using Rhino.Mocks;

namespace PodcastUtilities.Common.Tests.Files.SorterTests
{
	public abstract class WhenTestingTheSorter
		: WhenTestingBehaviour
	{
		protected Sorter FileSorter { get; set; }

		protected List<IFileInfo> OriginalFiles { get; set; }

		protected List<IFileInfo> SortedFiles { get; set; }

		protected override void GivenThat()
		{
			base.GivenThat();

			var file1 = GenerateMock<IFileInfo>();
			file1.Stub(f => f.Name).Return("aaa");
			file1.Stub(f => f.CreationTime).Return(new DateTime(2011, 2, 3));

			var file2 = GenerateMock<IFileInfo>();
			file2.Stub(f => f.Name).Return("zzz");
			file2.Stub(f => f.CreationTime).Return(new DateTime(1999, 7, 9));

			var file3 = GenerateMock<IFileInfo>();
			file3.Stub(f => f.Name).Return("mmm");
			file3.Stub(f => f.CreationTime).Return(new DateTime(2005, 10, 30));

			OriginalFiles = new List<IFileInfo>
			                	{
			                		file1,
			                		file2,
			                		file3
			                	};

			SortedFiles = new List<IFileInfo>(OriginalFiles);

			FileSorter = new Sorter();
		}
	}
}
