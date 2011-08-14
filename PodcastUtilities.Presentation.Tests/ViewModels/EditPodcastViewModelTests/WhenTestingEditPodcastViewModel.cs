using PodcastUtilities.Common;
using PodcastUtilities.Common.Tests;
using PodcastUtilities.Presentation.ViewModels;

namespace PodcastUtilities.Presentation.Tests.ViewModels.EditPodcastViewModelTests
{
	public abstract class WhenTestingEditPodcastViewModel
		: WhenTestingBehaviour
	{
		protected EditPodcastViewModel ViewModel { get; set; }

		protected PodcastInfo Podcast { get; set; }

		protected PodcastViewModel PodcastViewModel { get; set; }

		protected override void GivenThat()
		{
			base.GivenThat();

			Podcast = new PodcastInfo
			          	{
			          		Feed = new FeedInfo()
			          	};

			PodcastViewModel = new PodcastViewModel(Podcast);

			ViewModel = new EditPodcastViewModel(PodcastViewModel);
		}
	}
}