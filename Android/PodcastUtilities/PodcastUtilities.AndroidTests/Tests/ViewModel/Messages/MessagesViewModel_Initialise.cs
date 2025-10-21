using FakeItEasy;
using NUnit.Framework;
using PodcastUtilities.AndroidLogic.Logging;

namespace PodcastUtilities.AndroidTests.Tests.ViewModel.Messages
{
    [TestFixture]
    public class MessagesViewModel_Initialise : MessagesViewModelBase
    {
        [Test]
        public async void Initialise_Logs()
        {
            // arrange

            // act
            await ViewModel.Initialise().ConfigureAwait(false);

            // assert
            A.CallTo(() => MockLogger.Debug(A<ILogger.MessageGenerator>.Ignored)).MustHaveHappened(2, Times.Exactly);
        }

        [Test]
        public async void Initialise_SetsTheText()
        {
            // arrange
            SetupStore();

            // act
            await ViewModel.Initialise().ConfigureAwait(false);

            // assert
            Assert.AreEqual("line 1\nline 2\nline 3\n\n--- end of logs ---\n", ObservedResults.Messages?.ToString());
            Assert.AreEqual(1, ObservedResults.StartLoadingCount, "start loading");
            Assert.AreEqual(1, ObservedResults.EndLoadingCount, "end loading");
        }
    }
}