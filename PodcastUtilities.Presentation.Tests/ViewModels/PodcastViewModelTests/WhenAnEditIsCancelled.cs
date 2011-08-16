using System;
using NUnit.Framework;

namespace PodcastUtilities.Presentation.Tests.ViewModels.PodcastViewModelTests
{
    public class WhenAnEditIsCancelled
        : WhenTestingPodcastViewModel
    {
        protected override void GivenThat()
        {
            base.GivenThat();

            ViewModel.StartEditing();

            ViewModel.Name = "New name shouldn't be kept";
            ViewModel.Address = new Uri("http://www.newaddress.com/shouldbecancelled.xml");
        }

        protected override void When()
        {
            ViewModel.CancelEdit();
        }

        [Test]
        public void ItShouldRevertBackToTheOriginal()
        {
            Assert.That(ViewModel.Podcast.Folder, Is.EqualTo("Original Name"));
            Assert.That(ViewModel.Podcast.Feed.Address.AbsoluteUri, Is.EqualTo("http://www.originaladdress.com/ppp.xml"));
        }
    }
}