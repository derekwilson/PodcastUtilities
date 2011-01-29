namespace PodcastUtilities.Common.Tests.PodcastSynchronizerTests
{
	public abstract class WhenTestingPodcastSynchronizer
		: WhenTestingBehaviour
	{
		protected PodcastSynchronizer PodcastSynchronizer { get; set; }

		protected IControlFile ControlFile { get; set; }
		protected IFileFinder FileFinder { get; set; }
		protected IFileCopier FileCopier { get; set; }
		protected IUnwantedFileRemover FileRemover { get; set; }


		protected override void GivenThat()
		{
			base.GivenThat();

			ControlFile = GenerateMock<IControlFile>();
			FileFinder = GenerateMock<IFileFinder>();
			FileCopier = GenerateMock<IFileCopier>();
			FileRemover = GenerateMock<IUnwantedFileRemover>();

			PodcastSynchronizer = new PodcastSynchronizer(FileFinder, FileCopier, FileRemover);
		}
	}
}