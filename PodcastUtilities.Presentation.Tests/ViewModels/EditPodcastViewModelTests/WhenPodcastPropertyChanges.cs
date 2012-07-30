using System;
using NUnit.Framework;

namespace PodcastUtilities.Presentation.Tests.ViewModels.EditPodcastViewModelTests
{
	public class WhenPodcastPropertyChanges : WhenTestingEditPodcastViewModel
	{
		protected bool CanExecuteChangedFired { get; set; }

		protected override void GivenThat()
		{
			base.GivenThat();

			ViewModel.Podcast.Name = "blah";

			ViewModel.AcceptCommand.CanExecuteChanged += AcceptCommandCanExecuteChanged;
		}

		void AcceptCommandCanExecuteChanged(object sender, EventArgs e)
		{
			CanExecuteChangedFired = true;
		}

		protected override void When()
		{
			ViewModel.Podcast.Name = "new name";
		}

		[Test]
		public void ItShouldFireTheCanExecuteChangedEvent()
		{
			Assert.That(CanExecuteChangedFired, Is.True);
		}
	}
}