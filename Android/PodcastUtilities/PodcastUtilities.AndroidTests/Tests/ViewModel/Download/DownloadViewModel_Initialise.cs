using FakeItEasy;
using NUnit.Framework;
using PodcastUtilities.AndroidLogic.Logging;

namespace PodcastUtilities.AndroidTests.Tests.ViewModel.Download
{
    [TestFixture]
    public class DownloadViewModel_Initialise : DownloadViewModelBase
    {
        [Test]
        public void Initialise_Sets_Title()
        {
            // arrange

            // act
            ViewModel.Initialise(false);

            // assert
            Assert.AreEqual("Mocked Title", ObservedResults.LastSetTitle);
        }

        [Test]
        public void Initialise_Logs()
        {
            // arrange

            // act
            ViewModel.Initialise(false);

            // assert
            A.CallTo(() => MockLogger.Debug(A<ILogger.MessageGenerator>.Ignored)).MustHaveHappened(2, Times.Exactly);
        }
    }
}