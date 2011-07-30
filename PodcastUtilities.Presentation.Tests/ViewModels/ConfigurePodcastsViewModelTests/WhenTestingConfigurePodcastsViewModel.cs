using PodcastUtilities.Common;
using PodcastUtilities.Common.Tests;
using PodcastUtilities.Presentation.Services;
using PodcastUtilities.Presentation.ViewModels;

namespace PodcastUtilities.Presentation.Tests.ViewModels.ConfigurePodcastsViewModelTests
{
	public abstract class WhenTestingConfigurePodcastsViewModel
		: WhenTestingBehaviour
	{
		protected ConfigurePodcastsViewModel ViewModel { get; set; }

		protected IBrowseForFileService BrowseForFileService { get; set; }

		protected IControlFileFactory ControlFileFactory { get; set; }

		protected override void GivenThat()
		{
			base.GivenThat();

			BrowseForFileService = GenerateMock<IBrowseForFileService>();
			ControlFileFactory = GenerateMock<IControlFileFactory>();

			ViewModel = new ConfigurePodcastsViewModel(ControlFileFactory, BrowseForFileService);
		}
	}
}