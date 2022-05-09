using FakeItEasy;
using NUnit.Framework;
using PodcastUtilities.AndroidLogic.Logging;
using PodcastUtilities.AndroidTests.Helpers;
using PodcastUtilities.Common;
using PodcastUtilities.Common.Feeds;
using System;
using System.Collections.Generic;
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
            A.CallTo(() => MockCrashReporter.LogNonFatalException(A<Exception>.Ignored)).MustNotHaveHappened();
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
            A.CallTo(() => MockCrashReporter.LogNonFatalException(A<Exception>.Ignored)).MustNotHaveHappened();
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
            A.CallTo(() => MockCrashReporter.LogNonFatalException(A<Exception>.Ignored)).MustNotHaveHappened();
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
            A.CallTo(() => MockCrashReporter.LogNonFatalException(A<Exception>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => MockLogger.Warning(A<ILogger.MessageGenerator>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => MockTaskPool.RunAllTasks(A<int>.Ignored, A<ITask[]>.Ignored)).MustHaveHappened(2, Times.Exactly);
            Assert.AreEqual(2, ObservedResults.StartDownloadingCount, "start count");
            Assert.AreEqual(2, ObservedResults.EndDownloadingCount, "end count");
        }

        [Test]
        public void ProgressUpdate_ChecksFreeSpace()
        {
            // arrange
            SetupMockControlFileFor2Podcasts();
            SetupEpisodesFor2Podcasts();
            ViewModel.Initialise();
            ViewModel.FindEpisodesToDownload();

            SetupFireProgressEvent(EPISODE_1_ID, 9);

            // act
            ViewModel.DownloadAllPodcastsWithoutNetworkCheck().Wait();

            // assert
            A.CallTo(() => MockCrashReporter.LogNonFatalException(A<Exception>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => MockFileSystemHelper.GetAvailableFileSystemSizeInBytes(A<string>.Ignored)).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => MockTaskPool.CancelAllTasks()).MustNotHaveHappened();
        }

        [Test]
        public void ProgressUpdate_ChecksFreeSpace_Cancels_When_Disk_Full()
        {
            // arrange
            SetupMockControlFileFor2Podcasts();
            SetupEpisodesFor2Podcasts();
            ViewModel.Initialise();
            ViewModel.FindEpisodesToDownload();

            var syncItemMocker = SetupFireProgressEvent(EPISODE_1_ID, 9);
            // there is 1MB free in the filesystem
            A.CallTo(() => MockFileSystemHelper.GetAvailableFileSystemSizeInBytes(A<string>.Ignored)).Returns(1024 * 1024 * 1);

            // act
            ViewModel.DownloadAllPodcastsWithoutNetworkCheck().Wait();

            // assert
            A.CallTo(() => MockCrashReporter.LogNonFatalException(A<Exception>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => MockFileSystemHelper.GetAvailableFileSystemSizeInBytes(A<string>.Ignored)).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => MockTaskPool.CancelAllTasks()).MustHaveHappened(1, Times.Exactly);
        }

        [Test]
        public void ProgressUpdate_UpdatesProgress_At_10()
        {
            // arrange
            SetupMockControlFileFor2Podcasts();
            SetupEpisodesFor2Podcasts();
            ViewModel.Initialise();
            ViewModel.FindEpisodesToDownload();
            var syncItemMocker = SetupFireProgressEvent(EPISODE_1_ID, 10);
            Fake.ClearRecordedCalls(MockStatusAndProgressMessageStore);

            // act
            ViewModel.DownloadAllPodcastsWithoutNetworkCheck().Wait();

            // assert
            A.CallTo(() => MockCrashReporter.LogNonFatalException(A<Exception>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => MockStatusAndProgressMessageStore.StoreMessage(
                A<Guid>.That.Matches(g => g.ToString() == EPISODE_1_ID.ToString()),
                A<string>.That.Matches(s => s == "EpisodeTitle (10 bytes of 100 bytes) 10%")))
                .MustHaveHappened(1, Times.Exactly);
            Assert.AreEqual(10, ObservedResults.LastUpdatePercentage, "percentage");
            Assert.AreEqual(syncItemMocker.GetMockedSyncItem(), ObservedResults.LastUpdateItem, "item");
        }

        [Test]
        public void ProgressUpdate_UpdatesProgress_Not_At_11()
        {
            // arrange
            SetupMockControlFileFor2Podcasts();
            SetupEpisodesFor2Podcasts();
            ViewModel.Initialise();
            ViewModel.FindEpisodesToDownload();
            var syncItemMocker = SetupFireProgressEvent(EPISODE_1_ID, 11);
            Fake.ClearRecordedCalls(MockStatusAndProgressMessageStore);

            // act
            ViewModel.DownloadAllPodcastsWithoutNetworkCheck().Wait();

            // assert
            A.CallTo(() => MockCrashReporter.LogNonFatalException(A<Exception>.Ignored)).MustNotHaveHappened();
            A.CallTo(MockStatusAndProgressMessageStore).MustNotHaveHappened();
            Assert.AreEqual(0, ObservedResults.LastUpdatePercentage, "percentage");
            Assert.IsNull(ObservedResults.LastUpdateItem, "item");
        }

        [Test]
        public void ProgressUpdate_UpdatesProgress_At_100()
        {
            // arrange
            SetupMockControlFileFor2Podcasts();
            SetupEpisodesFor2Podcasts();
            ViewModel.Initialise();
            ViewModel.FindEpisodesToDownload();
            var syncItemMocker = SetupFireProgressEvent(EPISODE_1_ID, 100);
            Fake.ClearRecordedCalls(MockStatusAndProgressMessageStore);

            // act
            ViewModel.DownloadAllPodcastsWithoutNetworkCheck().Wait();

            // assert
            A.CallTo(() => MockCrashReporter.LogNonFatalException(A<Exception>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => MockStatusAndProgressMessageStore.StoreMessage(
                A<Guid>.That.Matches(g => g.ToString() == EPISODE_1_ID.ToString()),
                A<string>.That.Matches(s => s == "EpisodeTitle (100 bytes of 100 bytes) 100%")))
                .MustHaveHappened(1, Times.Exactly);
            Assert.AreEqual(100, ObservedResults.LastUpdatePercentage, "percentage");
            Assert.AreEqual(syncItemMocker.GetMockedSyncItem(), ObservedResults.LastUpdateItem, "item");
            A.CallTo(() => MockAnalyticsEngine.DownloadEpisodeEvent(A<long>.Ignored)).MustHaveHappened(1, Times.Exactly);
        }

        private SyncItemMocker SetupFireProgressEvent(Guid id, int percentage)
        {
            var syncItemMocker = new SyncItemMocker().ApplyId(id).ApplyEpisodeTitle("EpisodeTitle");
            EventHandler<ProgressEventArgs> progressEventHandler = null;
            ProgressEventArgs progressArgs = new ProgressEventArgs();
            A.CallTo(() =>
                MockSyncItemToEpisodeDownloaderTaskConverter.ConvertItemsToTasks
                (
                    A<IList<ISyncItem>>.Ignored,
                    A<EventHandler<StatusUpdateEventArgs>>.Ignored,
                    A<EventHandler<ProgressEventArgs>>.Ignored
                ))
                .ReturnsLazily((IList<ISyncItem> items, EventHandler<StatusUpdateEventArgs> statusEvent, EventHandler<ProgressEventArgs> progressEvent) =>
                {
                    progressEventHandler = progressEvent;
                    return new IEpisodeDownloader[0];
                });
            A.CallTo(() => MockTaskPool.RunAllTasks(A<int>.Ignored, A<ITask[]>.Ignored))
                .Invokes(() =>
                {
                    progressArgs.TotalItemsToProcess = 100;
                    progressArgs.ItemsProcessed = percentage;
                    progressArgs.ProgressPercentage = percentage;
                    progressArgs.UserState = syncItemMocker.GetMockedSyncItem();
                    progressEventHandler?.Invoke(this, progressArgs);
                });
            return syncItemMocker;
        }


    }
}