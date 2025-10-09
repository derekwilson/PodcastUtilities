using FakeItEasy;
using NUnit.Framework;
using PodcastUtilities.AndroidLogic.Logging;

namespace PodcastUtilities.AndroidTests.Tests.ViewModel.Help
{
    [TestFixture]
    public class HelpViewModel_Initialise : HelpViewModelBase
    {
        [Test]
        public void Initialise_Logs()
        {
            // arrange

            // act
            ViewModel.Initialise();

            // assert
            A.CallTo(() => MockLogger.Debug(A<ILogger.MessageGenerator>.Ignored)).MustHaveHappened(2, Times.Exactly);
        }

        [Test]
        public void Initialise_SetsTheText()
        {
            // arrange
            SetupHelpText();

            // act
            ViewModel.Initialise();

            // assert
            Assert.AreEqual("<h3>Test</h3><p>help text</p>", ObservedResults.LastSetText);
        }
    }
}