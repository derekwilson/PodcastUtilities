using NUnit.Framework;
using PodcastUtilities.Presentation.ViewModels;
using Rhino.Mocks;

namespace PodcastUtilities.Presentation.Tests.ViewModels.ConfigurePodcastsViewModelTests
{
	public class WhenExecutingEditPodcastCommand
		: WhenTestingConfigurePodcastsViewModel
	{
		public PodcastViewModel SelectedPodcast { get; set; }

		protected override void GivenThat()
		{
			base.GivenThat();

			SelectedPodcast = new PodcastViewModel(null);

			ViewModel.SelectedPodcast = SelectedPodcast;
		}

		protected override void When()
		{
			ViewModel.EditPodcastCommand.Execute(null);
		}

		[Test]
		public void ItShouldShowEditPodcastDialogForSelectedPodcast()
		{
			DialogService.AssertWasCalled(s => s.ShowEditPodcastDialog(SelectedPodcast));
		}
	}
}