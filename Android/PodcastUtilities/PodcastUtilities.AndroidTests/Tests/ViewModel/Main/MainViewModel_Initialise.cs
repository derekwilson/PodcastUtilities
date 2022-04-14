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
            ViewModel.Initialise();

            Assert.AreEqual("Mocked Title", LastSetTitle);
        }

        [Test]
        public void Initialise_Logs()
        {
            ViewModel.Initialise();

            A.CallTo(() => MockLogger.Debug(A<ILogger.MessageGenerator>.Ignored)).MustHaveHappened(2, Times.Exactly);
        }

        [Test]
        public void Initialise_NoControlFile_Handled()
        {
            ViewModel.Initialise();

            Assert.AreEqual("", LastSetCacheRoot);
            Assert.AreEqual("feed count == 0", LastSetFeedHeading);
        }
    }
}