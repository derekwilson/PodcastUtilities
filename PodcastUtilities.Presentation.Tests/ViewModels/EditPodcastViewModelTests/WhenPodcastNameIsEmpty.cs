using System;
using NUnit.Framework;

namespace PodcastUtilities.Presentation.Tests.ViewModels.EditPodcastViewModelTests
{
	public class WhenPodcastNameIsEmpty : WhenTestingEditPodcastViewModel
	{
		protected override void When()
		{
			ViewModel.Podcast.Name = String.Empty;
			ViewModel.Podcast.Address = new Uri("http://www.blah.com");
		}

		[Test]
		public void ItShouldStopExecutionOfAcceptCommand()
		{
			Assert.That(ViewModel.AcceptCommand.CanExecute(null), Is.False);
		}
	}
}