using System;
using NUnit.Framework;
using PodcastUtilities.Common;
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

            ViewModel.Podcasts.Add(new PodcastViewModel(new PodcastInfo()));

            CreatedPodcast = new PodcastInfo();

            PodcastFactory.Stub(f => f.CreatePodcast())
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

    public class WhenAddPodcastIsAccepted : WhenExecutingAddPodcastCommand
    {
        protected override bool EditPodcastDialogReturn
        {
            get { return true; }
        }

        [Test]
        public void ItShouldAddTheNewPodcastToTheCollection()
        {
            Assert.That(ViewModel.Podcasts.Count, Is.EqualTo(2));
            Assert.That(ViewModel.Podcasts[1], Is.SameAs(CreatedPodcastViewModel));
        }
    }

    public class WhenAddPodcastIsCancelled : WhenExecutingAddPodcastCommand
    {
        protected override bool EditPodcastDialogReturn
        {
            get { return false; }
        }

        [Test]
        public void ItShouldNotAddTheNewPodcastToTheCollection()
        {
            Assert.That(ViewModel.Podcasts.Count, Is.EqualTo(1));
            Assert.That(!ViewModel.Podcasts.Contains(CreatedPodcastViewModel));
        }
    }
}