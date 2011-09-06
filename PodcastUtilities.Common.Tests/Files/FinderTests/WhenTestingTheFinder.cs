using System.Collections.Generic;
using PodcastUtilities.Common.Files;
using PodcastUtilities.Common.Platform;
using Rhino.Mocks;

namespace PodcastUtilities.Common.Tests.Files.FinderTests
{
	public abstract class WhenTestingTheFinder
		: WhenTestingBehaviour
	{
		protected Finder FileFinder { get; set; }
		
		protected ISorter FileSorter { get; set; }
		protected IDirectoryInfoProvider DirectoryInfoProvider { get; set; }
		protected IDirectoryInfo DirectoryInfo { get; set; }

		protected IFileInfo[] FilesInDirectory { get; set; }

        protected IList<IFileInfo> FoundFiles { get; set; }


		protected override void GivenThat()
		{
			base.GivenThat();

			FileSorter = GenerateMock<ISorter>();
			DirectoryInfoProvider = GenerateMock<IDirectoryInfoProvider>();
			DirectoryInfo = GenerateMock<IDirectoryInfo>();

			FilesInDirectory = new IFileInfo[]
			                   	{
			                   		GenerateMock<IFileInfo>(),
									GenerateMock<IFileInfo>(),
									GenerateMock<IFileInfo>()
			                   	};

			DirectoryInfo.Stub(d => d.GetFiles(null))
				.IgnoreArguments()
				.Return(FilesInDirectory);

			DirectoryInfoProvider.Stub(d => d.GetDirectoryInfo(null))
				.IgnoreArguments()
				.Return(DirectoryInfo);

		    FileSorter.Stub(finder => finder.Sort(FilesInDirectory, "name", true)).Return(FilesInDirectory);

			FileFinder = new Finder(FileSorter, DirectoryInfoProvider);
		}
	}
}
