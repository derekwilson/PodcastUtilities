using NUnit.Framework;

namespace PodcastUtilities.Presentation.Tests.DelegateCommandTests
{
    public class WhenCallingCanExecuteCommand
        : WhenTestingDelegateCommand
    {
        public object TestParameter { get; set; }

        public bool CanExecuteReturn { get; set; }

        protected override void GivenThat()
        {
            base.GivenThat();

            TestParameter = new object();
        }

        protected override void When()
        {
            CanExecuteReturn = Command.CanExecute(TestParameter);
        }

        [Test]
        public void ItShouldCallTheDelegateWithSuppliedParameter()
        {
            Assert.That(CanExecuteParameter, Is.SameAs(TestParameter));
        }

        [Test]
        public void ItShouldUseTheDelegateReturnToDetermineCanExecuteStatus()
        {
            Assert.That(CanExecuteReturn, Is.True);
        }
    }
}