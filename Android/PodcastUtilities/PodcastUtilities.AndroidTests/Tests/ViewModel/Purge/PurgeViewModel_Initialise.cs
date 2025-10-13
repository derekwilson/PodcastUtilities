using FakeItEasy;
using NUnit.Framework;
using PodcastUtilities.AndroidLogic.Logging;

namespace PodcastUtilities.AndroidTests.Tests.ViewModel.Purge
{
    [TestFixture]
    public class PurgeViewModel_Initialise : PurgeViewModelBase
    {
        [Test]
        public void Initialise_Logs()
        {
            // arrange

            // act
            ViewModel.Initialise();

            // assert
            A.CallTo(() => MockLogger.Debug(A<ILogger.MessageGenerator>.Ignored)).MustHaveHappened(1, Times.Exactly);
        }

        [Test]
        public void Initialise_SetsTheTitle()
        {
            // arrange

            // act
            ViewModel.Initialise();

            // assert
            Assert.AreEqual("Mocked Title", ObservedResults.LastSetTitle);
        }
    }
}