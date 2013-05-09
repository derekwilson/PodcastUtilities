using System.Collections.Generic;
using PodcastUtilities.Common.Files;
using PodcastUtilities.Common.Platform;
using Rhino.Mocks;

namespace PodcastUtilities.Common.Tests.Files.UnwantedFileRemoverTests
{
	public abstract class WhenTestingUnwantedFileRemover
		: WhenTestingBehaviour
	{
		protected UnwantedFileRemover FileRemover { get; set; }

		protected IFileUtilities FileUtilities { get; set; }
		protected IDirectoryInfoProvider DirectoryInfoProvider { get; set; }
		protected IDirectoryInfo DirectoryInfo { get; set; }

		protected IFileInfo[] FilesInDirectory { get; set; }

		protected List<IFileInfo> FilesToKeep { get; set; }

		protected List<StatusUpdateEventArgs> StatusUpdates { get; set; }


		protected override void GivenThat()
		{
			base.GivenThat();

			FileUtilities = GenerateMock<IFileUtilities>();
			DirectoryInfoProvider = GenerateMock<IDirectoryInfoProvider>();
			DirectoryInfo = GenerateMock<IDirectoryInfo>();

			FilesInDirectory = new IFileInfo[]
			                   	{
			                   		GenerateMock<IFileInfo>(),
									GenerateMock<IFileInfo>(),
									GenerateMock<IFileInfo>()
			                   	};
			FilesInDirectory[0].Stub(f => f.FullName).Return(@"c:\a\b\c\1.mp3");
			FilesInDirectory[0].Stub(f => f.Name).Return(@"1.mp3");
			FilesInDirectory[1].Stub(f => f.FullName).Return(@"c:\a\b\c\2.mp3");
			FilesInDirectory[1].Stub(f => f.Name).Return(@"2.mp3");
			FilesInDirectory[2].Stub(f => f.FullName).Return(@"c:\a\b\c\3.mp3");
			FilesInDirectory[2].Stub(f => f.Name).Return(@"3.mp3");

			FilesToKeep = new List<IFileInfo>();

			DirectoryInfo.Stub(d => d.GetFiles(null))
				.IgnoreArguments()
				.Return(FilesInDirectory);

			DirectoryInfoProvider.Stub(d => d.GetDirectoryInfo(null))
				.IgnoreArguments()
				.Return(DirectoryInfo);

			StatusUpdates = new List<StatusUpdateEventArgs>();

			FileRemover = new UnwantedFileRemover(DirectoryInfoProvider, FileUtilities);
			FileRemover.StatusUpdate += FileRemoverStatusUpdate;
		}

		void FileRemoverStatusUpdate(object sender, StatusUpdateEventArgs e)
		{
			StatusUpdates.Add(e);
		}
	}
}
