using FakeItEasy;
using NUnit.Framework;
using PodcastUtilities.AndroidLogic.Logging;

namespace PodcastUtilities.AndroidTests.Tests.ViewModel.Main
{
    [TestFixture]
    public class MainViewModel_Initialise : MainViewModelBase
    {
        [Test]
        public void Initialise_Sets_Title()
        {
            // arrange

            // act
            ViewModel.Initialise();

            // assert
            Assert.AreEqual("Mocked Title", LastSetTitle);
        }

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
        public void Initialise_NoControlFile_Handled()
        {
            // arrange

            // act
            ViewModel.Initialise();

            // assert
            Assert.AreEqual("", LastSetCacheRoot);
            Assert.AreEqual("feed count == 0", LastSetFeedHeading);
            Assert.AreEqual(0, LastSetFeedItems.Count);
        }

        [Test]
        public void Initialise_ControlFile_1Pod_Handled()
        {
            // arrange
            SetupControlFileFor1Podcast();

            // act
            ViewModel.Initialise();

            // assert
            Assert.AreEqual("/sdcard/sourceroot", LastSetCacheRoot);
            Assert.AreEqual("feed count == 1", LastSetFeedHeading);
            Assert.AreEqual(1, LastSetFeedItems.Count);
        }
    }
}