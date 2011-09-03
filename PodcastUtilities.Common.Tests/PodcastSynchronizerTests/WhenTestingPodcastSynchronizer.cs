using PodcastUtilities.Common.Configuration;
using PodcastUtilities.Common.Files;

namespace PodcastUtilities.Common.Tests.PodcastSynchronizerTests
{
	public abstract class WhenTestingPodcastSynchronizer
		: WhenTestingBehaviour
	{
		protected PodcastSynchronizer PodcastSynchronizer { get; set; }

		protected IControlFile ControlFile { get; set; }
		protected IFinder FileFinder { get; set; }
		protected ICopier FileCopier { get; set; }
		protected IUnwantedFileRemover FileRemover { get; set; }


		protected override void GivenThat()
		{
			base.GivenThat();

			ControlFile = GenerateMock<IControlFile>();
			FileFinder = GenerateMock<IFinder>();
			FileCopier = GenerateMock<ICopier>();
			FileRemover = GenerateMock<IUnwantedFileRemover>();

			PodcastSynchronizer = new PodcastSynchronizer(FileFinder, FileCopier, FileRemover);
		}
	}
}