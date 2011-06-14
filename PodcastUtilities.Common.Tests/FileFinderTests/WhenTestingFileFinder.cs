using System.Collections.Generic;
using PodcastUtilities.Common.Platform;
using Rhino.Mocks;

namespace PodcastUtilities.Common.Tests.FileFinderTests
{
	public abstract class WhenTestingFileFinder
		: WhenTestingBehaviour
	{
		protected FileFinder FileFinder { get; set; }
		
		protected IFileSorter FileSorter { get; set; }
		protected IDirectoryInfoProvider DirectoryInfoProvider { get; set; }
		protected IDirectoryInfo DirectoryInfo { get; set; }

		protected IFileInfo[] FilesInDirectory { get; set; }

		protected List<IFileInfo> FoundFiles { get; set; }


		protected override void GivenThat()
		{
			base.GivenThat();

			FileSorter = GenerateMock<IFileSorter>();
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

			FileFinder = new FileFinder(FileSorter, DirectoryInfoProvider);
		}
	}
}
