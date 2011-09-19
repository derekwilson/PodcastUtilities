using System;
using NUnit.Framework;
using PodcastUtilities.Common;
using PodcastUtilities.Common.Configuration;
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

		    SelectedPodcast = GeneratePartialMock<PodcastViewModel>(
                new PodcastInfo(ControlFile)
                    {
                        Folder = "Original name",
                        Feed = new FeedInfo(ControlFile) { Address = new Uri("http://www.blah.com/rss.xml")}
                    });

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

		[Test]
		public void ItShouldStartEditingThePodcast()
		{
            SelectedPodcast.AssertWasCalled(p => p.StartEditing());
        }
	}
}