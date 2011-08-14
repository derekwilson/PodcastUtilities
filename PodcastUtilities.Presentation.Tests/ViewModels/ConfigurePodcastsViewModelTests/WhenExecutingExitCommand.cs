using NUnit.Framework;
using Rhino.Mocks;

namespace PodcastUtilities.Presentation.Tests.ViewModels.ConfigurePodcastsViewModelTests
{
	public class WhenExecutingExitCommand
		: WhenTestingConfigurePodcastsViewModel
	{
		protected override void When()
		{
			ViewModel.ExitCommand.Execute(null);
		}

		[Test]
		public void ItShouldShutdownTheApplication()
		{
			ApplicationService.AssertWasCalled(s => s.ShutdownApplication());
		}
	}
}