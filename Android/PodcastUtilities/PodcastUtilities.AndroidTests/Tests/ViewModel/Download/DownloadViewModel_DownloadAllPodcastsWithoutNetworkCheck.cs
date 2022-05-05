using FakeItEasy;
using NUnit.Framework;
using PodcastUtilities.AndroidLogic.Logging;
using PodcastUtilities.Common;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PodcastUtilities.AndroidTests.Tests.ViewModel.Download
{
    [TestFixture]
    public class DownloadViewModel_DownloadAllPodcastsWithoutNetworkCheck : DownloadViewModelBase
    {
        [Test]
        public async Task DownloadAllPodcastsWithoutNetworkCheck_HandlesNoDownloads()
        {
            // arrange
            ViewModel.Initialise();

            // act
            await ViewModel.DownloadAllPodcastsWithoutNetworkCheck().ConfigureAwait(false);

            // assert
            Assert.AreEqual("No downloads", ObservedResults.LastDisplayMessage);
        }

        [Test]
        public async Task DownloadAllPodcastsWithoutNetworkCheck_HandlesException()
        {
            // arrange
            SetupMockControlFileFor2Podcasts();
            SetupEpisodesFor2Podcasts();
            ViewModel.Initialise();
            ViewModel.FindEpisodesToDownload();
            var testException = new Exception("TEST EXCEPTION");
            A.CallTo(() => MockTaskPool.RunAllTasks(A<int>.Ignored, A<ITask[]>.Ignored)).Throws(testException);

            // act
            await ViewModel.DownloadAllPodcastsWithoutNetworkCheck().ConfigureAwait(false);

            // assert
            Assert.AreEqual(1, ObservedResults.StartDownloadingCount, "start count");
            Assert.AreEqual(1, ObservedResults.EndDownloadingCount, "end count");
            A.CallTo(() => MockCrashReporter.LogNonFatalException(testException)).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => MockLogger.LogException(A<ILogger.MessageGenerator>.Ignored, testException)).MustHaveHappened(1, Times.Exactly);
            Assert.AreEqual("TEST EXCEPTION", ObservedResults.LastDisplayMessage);
        }

        [Test]
        public async Task DownloadAllPodcastsWithoutNetworkCheck_StartsAndStops()
        {
            // arrange
            SetupMockControlFileFor2Podcasts();
            SetupEpisodesFor2Podcasts();
            ViewModel.Initialise();
            ViewModel.FindEpisodesToDownload();

            // act
            await ViewModel.DownloadAllPodcastsWithoutNetworkCheck().ConfigureAwait(false);

            // assert
            Assert.AreEqual(1, ObservedResults.StartDownloadingCount, "start count");
            Assert.AreEqual(1, ObservedResults.EndDownloadingCount, "end count");
        }

        [Test]
        public async Task DownloadAllPodcastsWithoutNetworkCheck_SetsMaxDownloads()
        {
            // arrange
            SetupMockControlFileFor2Podcasts();
            SetupEpisodesFor2Podcasts();
            ViewModel.Initialise();
            ViewModel.FindEpisodesToDownload();

            // act
            await ViewModel.DownloadAllPodcastsWithoutNetworkCheck().ConfigureAwait(false);

            // assert
            A.CallTo(() => MockNetworkHelper.SetNetworkConnectionLimit(MAX_DOWNLOADS)).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => MockTaskPool.RunAllTasks(MAX_DOWNLOADS, A<ITask[]>.Ignored)).MustHaveHappened(1, Times.Exactly);
        }

        [Test]
        public void DownloadAllPodcastsWithoutNetworkCheck_HandlesMultipleCalls_Concurrent()
        {
            // arrange
            int taskStartCount = 0;
            SetupMockControlFileFor2Podcasts();
            SetupEpisodesFor2Podcasts();
            ViewModel.Initialise();
            ViewModel.FindEpisodesToDownload();
            A.CallTo(() => MockTaskPool.RunAllTasks(A<int>.Ignored, A<ITask[]>.Ignored))
                .Invokes(() =>
                { 
                    taskStartCount++;
                    if (taskStartCount > 1)
                    {
                        throw new Exception("Concurrent calls are not being trapped");
                    }
                    ViewModel.DownloadAllPodcastsWithoutNetworkCheck().Wait();
                });

            // act
            ViewModel.DownloadAllPodcastsWithoutNetworkCheck().Wait();

            // assert
            A.CallTo(() => MockLogger.Warning(A<ILogger.MessageGenerator>.Ignored)).MustHaveHappened(1, Times.Exactly);
            Assert.AreEqual(1, ObservedResults.StartDownloadingCount, "start count");
            Assert.AreEqual(1, ObservedResults.EndDownloadingCount, "end count");
            Assert.AreEqual(1, taskStartCount, "task start count");
        }

        [Test]
        public void DownloadAllPodcastsWithoutNetworkCheck_HandlesMultipleCalls_Sequential()
        {
            // arrange
            SetupMockControlFileFor2Podcasts();
            SetupEpisodesFor2Podcasts();
            ViewModel.Initialise();
            ViewModel.FindEpisodesToDownload();

            // act
            ViewModel.DownloadAllPodcastsWithoutNetworkCheck().Wait();
            ViewModel.DownloadAllPodcastsWithoutNetworkCheck().Wait();

            // assert
            A.CallTo(() => MockLogger.Warning(A<ILogger.MessageGenerator>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => MockTaskPool.RunAllTasks(A<int>.Ignored, A<ITask[]>.Ignored)).MustHaveHappened(2, Times.Exactly);
            Assert.AreEqual(2, ObservedResults.StartDownloadingCount, "start count");
            Assert.AreEqual(2, ObservedResults.EndDownloadingCount, "end count");
        }
    }
}