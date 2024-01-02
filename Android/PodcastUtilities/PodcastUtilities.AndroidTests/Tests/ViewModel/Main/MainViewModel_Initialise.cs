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
            Assert.AreEqual("Mocked Title", ObservedResults.LastSetTitle);
        }

        [Test]
        public void Initialise_Logs()
        {
            // arrange

            // act
            ViewModel.Initialise();

            // assert
            A.CallTo(() => MockLogger.Debug(A<ILogger.MessageGenerator>.Ignored)).MustHaveHappened(3, Times.Exactly);
        }

        [Test]
        public void Initialise_NoControlFile_Handled()
        {
            // arrange

            // act
            ViewModel.Initialise();

            // assert
            Assert.AreEqual("", ObservedResults.LastSetCacheRoot);
            Assert.AreEqual("feed count == 0", ObservedResults.LastSetFeedHeading);
            Assert.AreEqual(0, ObservedResults.LastSetFeedItems.Count);
        }

        [Test]
        public void Initialise_ControlFile_1Pod_Handled()
        {
            // arrange
            SetupMockControlFileFor1Podcast();

            // act
            ViewModel.Initialise();

            // assert
            Assert.AreEqual("/sdcard/sourceroot", ObservedResults.LastSetCacheRoot);
            Assert.AreEqual("feed count == 1", ObservedResults.LastSetFeedHeading);
            Assert.AreEqual(1, ObservedResults.LastSetFeedItems.Count);
            Assert.AreEqual("folder1", ObservedResults.LastSetFeedItems[0].PodcastFeed.Folder);
        }
    }
}