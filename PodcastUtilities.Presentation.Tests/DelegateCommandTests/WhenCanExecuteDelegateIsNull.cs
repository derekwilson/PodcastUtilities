using NUnit.Framework;

namespace PodcastUtilities.Presentation.Tests.DelegateCommandTests
{
	public class WhenCanExecuteDelegateIsNull
		: WhenTestingDelegateCommand
	{
		public bool CanExecuteReturn { get; set; }

		protected override void GivenThat()
		{
			base.GivenThat();

			Command = new DelegateCommand(parameter => ExecuteParameter = parameter);
		}

		protected override void When()
		{
			CanExecuteReturn = Command.CanExecute(null);
		}

		[Test]
		public void ItShouldReturnTrueFromCanExecute()
		{
			Assert.That(CanExecuteReturn, Is.True);
		}
	}
}