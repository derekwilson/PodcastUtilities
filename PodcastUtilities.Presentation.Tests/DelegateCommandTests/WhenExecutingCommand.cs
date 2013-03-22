using NUnit.Framework;

namespace PodcastUtilities.Presentation.Tests.DelegateCommandTests
{
    public class WhenExecutingCommand
        : WhenTestingDelegateCommand
    {
        public object TestParameter { get; set; }

        protected override void GivenThat()
        {
            base.GivenThat();

            TestParameter = new object();
        }

        protected override void When()
        {
            Command.Execute(TestParameter);
        }

        [Test]
        public void ItShouldCallTheDelegateWithSuppliedParameter()
        {
            Assert.That(ExecuteParameter, Is.SameAs(TestParameter));
        }
    }
}