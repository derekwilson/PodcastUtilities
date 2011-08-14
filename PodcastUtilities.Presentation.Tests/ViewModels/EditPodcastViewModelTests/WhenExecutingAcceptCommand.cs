using NUnit.Framework;

namespace PodcastUtilities.Presentation.Tests.ViewModels.EditPodcastViewModelTests
{
	public class WhenExecutingAcceptCommand : WhenTestingEditPodcastViewModel
	{
		protected override void When()
		{
			ViewModel.AcceptCommand.Execute(null);
		}

		[Test]
		public void ItShouldSetDialogResultToTrue()
		{
			Assert.That(ViewModel.DialogResult, Is.True);
		}
	}
}