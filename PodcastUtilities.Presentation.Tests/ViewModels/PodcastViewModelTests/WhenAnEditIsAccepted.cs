using System;
using NUnit.Framework;

namespace PodcastUtilities.Presentation.Tests.ViewModels.PodcastViewModelTests
{
    public class WhenAnEditIsAccepted
        : WhenTestingPodcastViewModel
    {
        protected override void GivenThat()
        {
            base.GivenThat();

            ViewModel.StartEditing();

            ViewModel.Name = "New name";
            ViewModel.Address = new Uri("http://www.newaddress.com/podcast.xml");
        }

        protected override void When()
        {
            ViewModel.AcceptEdit();
        }

        [Test]
        public void ItShouldRetainTheChanges()
        {
            Assert.That(ViewModel.Podcast.Folder, Is.EqualTo("New name"));
            Assert.That(ViewModel.Podcast.Feed.Address.AbsoluteUri, Is.EqualTo("http://www.newaddress.com/podcast.xml"));
        }
    }
}