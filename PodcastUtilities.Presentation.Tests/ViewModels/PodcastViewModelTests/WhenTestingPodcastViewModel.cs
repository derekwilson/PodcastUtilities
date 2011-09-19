using System;
using PodcastUtilities.Common;
using PodcastUtilities.Common.Configuration;
using PodcastUtilities.Common.Tests;
using PodcastUtilities.Presentation.ViewModels;

namespace PodcastUtilities.Presentation.Tests.ViewModels.PodcastViewModelTests
{
    public abstract class WhenTestingPodcastViewModel
        : WhenTestingBehaviour
    {
        protected PodcastViewModel ViewModel { get; set; }

        protected IReadOnlyControlFile ControlFile { get; set; }

        protected override void GivenThat()
        {
            base.GivenThat();

            ControlFile = GenerateMock<IReadOnlyControlFile>();

            var podcast = new PodcastInfo(ControlFile)
                              {
                                  Folder = "Original Name",
                                  Feed = new FeedInfo(ControlFile) { Address = new Uri("http://www.originaladdress.com/ppp.xml") }
                              };

            ViewModel = new PodcastViewModel(podcast);
        }
    }
}