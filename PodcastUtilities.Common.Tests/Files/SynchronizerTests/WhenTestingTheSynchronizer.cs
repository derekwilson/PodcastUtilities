using PodcastUtilities.Common.Configuration;
using PodcastUtilities.Common.Files;

namespace PodcastUtilities.Common.Tests.Files.SynchronizerTests
{
	public abstract class WhenTestingTheSynchronizer
		: WhenTestingBehaviour
	{
		protected Synchronizer PodcastSynchronizer { get; set; }

		protected IReadOnlyControlFile ControlFile { get; set; }
		protected IFinder FileFinder { get; set; }
		protected ICopier FileCopier { get; set; }
		protected IUnwantedFileRemover FileRemover { get; set; }


		protected override void GivenThat()
		{
			base.GivenThat();

			ControlFile = GenerateMock<IReadOnlyControlFile>();
			FileFinder = GenerateMock<IFinder>();
			FileCopier = GenerateMock<ICopier>();
			FileRemover = GenerateMock<IUnwantedFileRemover>();

			PodcastSynchronizer = new Synchronizer(FileFinder, FileCopier, FileRemover);
		}
	}
}