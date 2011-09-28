using System;
using NUnit.Framework;
using PodcastUtilities.Common;
using PodcastUtilities.Common.Configuration;
using PodcastUtilities.Presentation.ViewModels;

namespace PodcastUtilities.Presentation.Tests.ViewModels.PodcastViewModelTests
{
    public class WhenStartingAnEditOnANewPodcast
        : WhenTestingPodcastViewModel
    {
        protected override void GivenThat()
        {
            base.GivenThat();

            ViewModel = new PodcastViewModel(new PodcastInfo(ControlFile) {
                Folder = "New Name",
                Feed = new FeedInfo(ControlFile) { Address = new Uri("http://www.newaddress.com/ppp.xml") }
            });
        }

        protected override void When()
        {
            ViewModel.StartEditing();
        }

        [Test]
        public void ItShouldThenBeInEditingState()
        {
            Assert.That(ViewModel.IsEditing, Is.True);
        }
    }
}