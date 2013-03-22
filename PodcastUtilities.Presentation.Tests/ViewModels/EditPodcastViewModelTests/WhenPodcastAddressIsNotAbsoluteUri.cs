using System;
using NUnit.Framework;

namespace PodcastUtilities.Presentation.Tests.ViewModels.EditPodcastViewModelTests
{
    public class WhenPodcastAddressIsNotAbsoluteUri : WhenTestingEditPodcastViewModel
    {
        protected override void When()
        {
            ViewModel.Podcast.Name = "Blah";
            ViewModel.Podcast.Address = new Uri("fred/fred", UriKind.Relative);
        }

        [Test]
        public void ItShouldStopExecutionOfAcceptCommand()
        {
            Assert.That(ViewModel.AcceptCommand.CanExecute(null), Is.False);
        }
    }
}