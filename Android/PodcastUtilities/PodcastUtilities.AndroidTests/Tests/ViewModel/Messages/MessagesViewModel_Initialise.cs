using FakeItEasy;
using NUnit.Framework;
using PodcastUtilities.AndroidLogic.Logging;

namespace PodcastUtilities.AndroidTests.Tests.ViewModel.Messages
{
    [TestFixture]
    public class MessagesViewModel_Initialise : MessagesViewModelBase
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
            SetupStore();

            // act
            ViewModel.Initialise();

            // assert
            Assert.AreEqual("line 1\nline 2\nline 3\n", ObservedResults.Messages.ToString());
            Assert.AreEqual(1, ObservedResults.ScrollToTopCount);
        }
    }
}