using FakeItEasy;
using NUnit.Framework;
using System;

namespace PodcastUtilities.AndroidTests.Tests.ViewModel.Main
{
    [TestFixture]
    public class MainViewModel_LogLevelUpdated : MainViewModelBase
    {
        [Test]
        public void Initialise_ControlFile_Sets_Log_Level()
        {
            // arrange
            SetupMockControlFileFor1Podcast();
            ViewModel.Initialise();

            // act
            Fake.ClearRecordedCalls(MockAndroidApplication);
            MockApplicationControlFileProvider.LoggingLevelUpdated += Raise.With(EventArgs.Empty);

            // assert
            A.CallTo(() => MockAndroidApplication.SetLoggingNone()).MustHaveHappened(1, Times.Exactly);
        }
    }
}