using System;
using NUnit.Framework;

namespace PodcastUtilities.Presentation.Tests.ViewModels.EditPodcastViewModelTests
{
	public class WhenPodcastNameAndAddressAreBothValid : WhenTestingEditPodcastViewModel
	{
		protected override void When()
		{
			ViewModel.Podcast.Name = "Blah";
			ViewModel.Podcast.Address = new Uri("http://www.blah.com");
		}

		[Test]
		public void ItShouldAllowExecutionOfAcceptCommand()
		{
			Assert.That(ViewModel.AcceptCommand.CanExecute(null), Is.True);
		}
	}
}