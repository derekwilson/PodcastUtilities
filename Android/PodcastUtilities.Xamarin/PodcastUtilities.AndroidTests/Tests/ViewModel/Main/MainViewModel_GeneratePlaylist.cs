using FakeItEasy;
using NUnit.Framework;
using PodcastUtilities.Common;
using PodcastUtilities.Common.Configuration;
using PodcastUtilities.Common.Playlists;
using System;

namespace PodcastUtilities.AndroidTests.Tests.ViewModel.Main
{
    [TestFixture]
    public class MainViewModel_GeneratePlaylist : MainViewModelBase
    {
        [Test]
        public void GeneratePlaylist_CallsGenerate()
        {
            // arrange
            SetupMockControlFileFor1Podcast();
            ViewModel.Initialise();

            // act
            ViewModel.GeneratePlaylist();

            // assert
            A.CallTo(() => MockCrashReporter.LogNonFatalException(A<Exception>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => MockPlaylistGenerator.GeneratePlaylist(MockControlFile, "/sdcard/sourceroot", true, A<EventHandler<StatusUpdateEventArgs>>.Ignored)).MustHaveHappened(1, Times.Exactly);
        }

        [Test]
        public void GeneratePlaylist_RecordsAnalytics()
        {
            // arrange
            SetupMockControlFileFor1Podcast();
            ViewModel.Initialise();

            // act
            ViewModel.GeneratePlaylist();

            // assert
            A.CallTo(() => MockAnalyticsEngine.GeneratePlaylistEvent(PlaylistFormat.M3U)).MustHaveHappened(1, Times.Exactly);
        }

        [Test]
        public void GeneratePlaylist_StatusUpdates()
        {
            // arrange
            SetupMockControlFileFor1Podcast();
            ViewModel.Initialise();
            A.CallTo(() => MockPlaylistGenerator.GeneratePlaylist(MockControlFile, "/sdcard/sourceroot", true, A<EventHandler<StatusUpdateEventArgs>>.Ignored))
                                   .Invokes((IReadOnlyControlFile control, string root, bool copy, EventHandler<StatusUpdateEventArgs> statusUpdateHandler) =>
                                   {
                                       MockPlaylistGenerator.StatusUpdate += Raise.With(new StatusUpdateEventArgs(StatusUpdateLevel.Status, "status message", true, null));
                                   });


            // act
            ViewModel.GeneratePlaylist();

            // assert
            A.CallTo(() => MockCrashReporter.LogNonFatalException(A<Exception>.Ignored)).MustNotHaveHappened();
            Assert.AreEqual("status message", ObservedResults.LastToastMessage);
        }

        [Test]
        public void GeneratePlaylist_HandlesErrors()
        {
            // arrange
            SetupMockControlFileFor1Podcast();
            ViewModel.Initialise();
            var testException = new Exception();
            A.CallTo(() => MockPlaylistGenerator.GeneratePlaylist(A<IReadOnlyControlFile>.Ignored, A<string>.Ignored, A<bool>.Ignored, A<EventHandler<StatusUpdateEventArgs>>.Ignored)).Throws(testException);

            // act
            ViewModel.GeneratePlaylist();

            // assert
            A.CallTo(() => MockCrashReporter.LogNonFatalException(testException)).MustHaveHappened(1, Times.Exactly);
            Assert.AreEqual("Mocked playlist error", ObservedResults.LastToastMessage);
        }

    }
}