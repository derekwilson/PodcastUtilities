using FakeItEasy;
using NUnit.Framework;
using PodcastUtilities.AndroidLogic.Logging;

namespace PodcastUtilities.AndroidTests.Tests.ViewModel.Settings
{
    [TestFixture]
    public class OpenSourceLicensesViewModel_Initialise : OpenSourceLicensesViewModelBase
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
            SetupLicenseText();

            // act
            ViewModel.Initialise();

            // assert
            Assert.AreEqual("License header\nLicense body line 1\nLicense body line 2\n", ObservedResults.Text?.ToString());
            Assert.AreEqual(1, ObservedResults.ScrollToTopCount);
        }
    }
}