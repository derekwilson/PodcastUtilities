using System;
using NUnit.Framework;
using PodcastUtilities.Common;
using PodcastUtilities.Common.Configuration;
using PodcastUtilities.Presentation.ViewModels;
using Rhino.Mocks;

namespace PodcastUtilities.Presentation.Tests.ViewModels.ConfigurePodcastsViewModelTests
{
    public class WhenExecutingAddPodcastCommand
        : WhenTestingConfigurePodcastsViewModel
    {
        public PodcastViewModel CreatedPodcastViewModel { get; set; }

        public PodcastInfo CreatedPodcast { get; set; }

        protected virtual bool EditPodcastDialogReturn
        {
            get { return false; }
        }

        protected override void GivenThat()
        {
            base.GivenThat();

            ViewModel.Podcasts.Add(new PodcastViewModel(new PodcastInfo(ControlFile)));

            CreatedPodcast = new PodcastInfo(ControlFile)
                                 {
                                     Folder = "created",
                                     Feed = new FeedInfo(ControlFile)
                                 };

            PodcastFactory.Stub(f => f.CreatePodcast(null))
                .Return(CreatedPodcast);

            DialogService.Stub(s => s.ShowEditPodcastDialog(null))
                .IgnoreArguments()
                .WhenCalled(invocation => CreatedPodcastViewModel = (PodcastViewModel) invocation.Arguments[0])
                .Return(EditPodcastDialogReturn);
        }

        protected override void When()
        {
            ViewModel.AddPodcastCommand.Execute(null);
        }

        [Test]
        public void ItShouldCreatePodcastAndShowEditPodcastDialog()
        {
            DialogService.AssertWasCalled(s => s.ShowEditPodcastDialog(Arg<PodcastViewModel>.Matches(p => p.Podcast == CreatedPodcast)));
        }
    }
}