using FakeItEasy;
using NUnit.Framework;
using PodcastUtilities.AndroidLogic.Logging;
using PodcastUtilities.AndroidTests.Helpers;
using PodcastUtilities.AndroidLogic.ViewModel.Download;
using PodcastUtilities.Common;
using PodcastUtilities.Common.Feeds;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PodcastUtilities.Common.Configuration;

namespace PodcastUtilities.AndroidTests.Tests.ViewModel.Download
{
    [TestFixture]
    public class DownloadViewModel_DownloadAllPodcastsWithoutNetworkCheck : DownloadViewModelBase
    {
        [Test]
        public async Task DownloadAllPodcastsWithoutNetworkCheck_HandlesNoDownloads()
        {
            // arrange
            ViewModel.Initialise(false);

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
            ViewModel.Initialise(false);
            ViewModel.FindEpisodesToDownload(null);
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
            ViewModel.Initialise(false);
            ViewModel.FindEpisodesToDownload(null);

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
            ViewModel.Initialise(false);
            ViewModel.FindEpisodesToDownload(null);

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
            ViewModel.Initialise(false);
            ViewModel.FindEpisodesToDownload(null);
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
            ViewModel.Initialise(false);
            ViewModel.FindEpisodesToDownload(null);

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
            ViewModel.Initialise(false);
            ViewModel.FindEpisodesToDownload(null);

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
            ViewModel.Initialise(false);
            ViewModel.FindEpisodesToDownload(null);

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
            ViewModel.Initialise(false);
            ViewModel.FindEpisodesToDownload(null);
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
            Assert.AreEqual(syncItemMocker.GetMockedSyncItem(), ObservedResults.LastUpdatePercentageItem, "item");
        }

        [Test]
        public void ProgressUpdate_UpdatesProgress_Not_At_11()
        {
            // arrange
            SetupMockControlFileFor2Podcasts();
            SetupEpisodesFor2Podcasts();
            ViewModel.Initialise(false);
            ViewModel.FindEpisodesToDownload(null);
            var syncItemMocker = SetupFireProgressEvent(EPISODE_1_ID, 11);
            Fake.ClearRecordedCalls(MockStatusAndProgressMessageStore);

            // act
            ViewModel.DownloadAllPodcastsWithoutNetworkCheck().Wait();

            // assert
            A.CallTo(() => MockCrashReporter.LogNonFatalException(A<Exception>.Ignored)).MustNotHaveHappened();
            A.CallTo(MockStatusAndProgressMessageStore).MustNotHaveHappened();
            Assert.AreEqual(0, ObservedResults.LastUpdatePercentage, "percentage");
            Assert.IsNull(ObservedResults.LastUpdatePercentageItem, "item");
        }

        [Test]
        public void ProgressUpdate_UpdatesProgress_At_100()
        {
            // arrange
            SetupMockControlFileFor2Podcasts();
            SetupEpisodesFor2Podcasts();
            ViewModel.Initialise(false);
            ViewModel.FindEpisodesToDownload(null);
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
            Assert.AreEqual(syncItemMocker.GetMockedSyncItem(), ObservedResults.LastUpdatePercentageItem, "item");
            A.CallTo(() => MockAnalyticsEngine.DownloadEpisodeEvent(A<long>.Ignored)).MustHaveHappened(1, Times.Exactly);
        }

        [Test]
        public void StatusUpdate_StatusMessage()
        {
            // arrange
            SetupMockControlFileFor2Podcasts();
            SetupEpisodesFor2Podcasts();
            ViewModel.Initialise(false);
            ViewModel.FindEpisodesToDownload(null);
            var syncItemMocker = SetupFireStatusEvent(EPISODE_1_ID, StatusUpdateLevel.Status, false, null, "test message");
            Fake.ClearRecordedCalls(MockStatusAndProgressMessageStore);
            Fake.ClearRecordedCalls(MockAnalyticsEngine);

            // act
            ViewModel.DownloadAllPodcastsWithoutNetworkCheck().Wait();

            // assert
            A.CallTo(() => MockCrashReporter.LogNonFatalException(A<Exception>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => MockStatusAndProgressMessageStore.StoreMessage(
                A<Guid>.That.Matches(g => g.ToString() == EPISODE_1_ID.ToString()),
                A<string>.That.Matches(s => s == "test message")))
                .MustHaveHappened(1, Times.Exactly);
            Assert.AreEqual("test message", ObservedResults.LastUpdateStatusMessage, "message");
            Assert.AreEqual(syncItemMocker.GetMockedSyncItem(), ObservedResults.LastUpdateStatusItem, "item");
            Assert.AreEqual(Status.Information, ObservedResults.LastUpdateStatus, "status");
            A.CallTo(MockAnalyticsEngine).MustNotHaveHappened();
        }

        [Test]
        public void StatusUpdate_StatusMessageVerboseDisplays()
        {
            // arrange
            SetupMockControlFileFor2Podcasts(DiagnosticOutputLevel.Verbose);
            SetupEpisodesFor2Podcasts();
            ViewModel.Initialise(false);
            ViewModel.FindEpisodesToDownload(null);
            var syncItemMocker = SetupFireStatusEvent(EPISODE_1_ID, StatusUpdateLevel.Verbose, false, null, "test verbose");
            Fake.ClearRecordedCalls(MockStatusAndProgressMessageStore);
            Fake.ClearRecordedCalls(MockAnalyticsEngine);

            // act
            ViewModel.DownloadAllPodcastsWithoutNetworkCheck().Wait();

            // assert
            A.CallTo(() => MockCrashReporter.LogNonFatalException(A<Exception>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => MockStatusAndProgressMessageStore.StoreMessage(
                A<Guid>.That.Matches(g => g.ToString() == EPISODE_1_ID.ToString()),
                A<string>.That.Matches(s => s == "test verbose")))
                .MustHaveHappened(1, Times.Exactly);
            Assert.AreEqual("test verbose", ObservedResults.LastUpdateStatusMessage, "message");
            Assert.AreEqual(syncItemMocker.GetMockedSyncItem(), ObservedResults.LastUpdateStatusItem, "item");
            Assert.AreEqual(Status.Information, ObservedResults.LastUpdateStatus, "status");
            A.CallTo(MockAnalyticsEngine).MustNotHaveHappened();
        }

        [Test]
        public void StatusUpdate_StatusMessageVerboseDoesNotDisplay()
        {
            // arrange
            // verbose is not enabled
            SetupMockControlFileFor2Podcasts();
            SetupEpisodesFor2Podcasts();
            ViewModel.Initialise(false);
            ViewModel.FindEpisodesToDownload(null);
            var syncItemMocker = SetupFireStatusEvent(EPISODE_1_ID, StatusUpdateLevel.Verbose, false, null, "test verbose");
            Fake.ClearRecordedCalls(MockStatusAndProgressMessageStore);
            Fake.ClearRecordedCalls(MockAnalyticsEngine);
            Fake.ClearRecordedCalls(MockLogger);

            // act
            ViewModel.DownloadAllPodcastsWithoutNetworkCheck().Wait();

            // assert
            A.CallTo(() => MockCrashReporter.LogNonFatalException(A<Exception>.Ignored)).MustNotHaveHappened();
            Assert.IsNull(ObservedResults.LastUpdateStatusMessage, "message");
            Assert.IsNull(ObservedResults.LastUpdateStatusItem, "item");
            Assert.AreEqual(Status.OK, ObservedResults.LastUpdateStatus, "status");
            A.CallTo(MockStatusAndProgressMessageStore).MustNotHaveHappened();
            A.CallTo(MockAnalyticsEngine).MustNotHaveHappened();
            // but we have logged it
            A.CallTo(() => MockLogger.Verbose(A<ILogger.MessageGenerator>.Ignored)).MustHaveHappened(1, Times.Exactly);
        }

        [Test]
        public void StatusUpdate_StatusMessageComplete()
        {
            // arrange
            SetupMockControlFileFor2Podcasts();
            SetupEpisodesFor2Podcasts();
            ViewModel.Initialise(false);
            ViewModel.FindEpisodesToDownload(null);
            var syncItemMocker = SetupFireStatusEvent(EPISODE_1_ID, StatusUpdateLevel.Status, true, null, "test message complete");
            Fake.ClearRecordedCalls(MockStatusAndProgressMessageStore);
            Fake.ClearRecordedCalls(MockAnalyticsEngine);

            // act
            ViewModel.DownloadAllPodcastsWithoutNetworkCheck().Wait();

            // assert
            A.CallTo(() => MockCrashReporter.LogNonFatalException(A<Exception>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => MockStatusAndProgressMessageStore.StoreMessage(
                A<Guid>.That.Matches(g => g.ToString() == EPISODE_1_ID.ToString()),
                A<string>.That.Matches(s => s == "test message complete")))
                .MustHaveHappened(1, Times.Exactly);
            Assert.AreEqual("test message complete", ObservedResults.LastUpdateStatusMessage, "message");
            Assert.AreEqual(syncItemMocker.GetMockedSyncItem(), ObservedResults.LastUpdateStatusItem, "item");
            Assert.AreEqual(Status.Complete, ObservedResults.LastUpdateStatus, "status");
            A.CallTo(() => MockAnalyticsEngine.DownloadEpisodeCompleteEvent()).MustHaveHappened(1, Times.Exactly);
        }

        [Test]
        public void StatusUpdate_Exception()
        {
            // arrange
            SetupMockControlFileFor2Podcasts();
            SetupEpisodesFor2Podcasts();
            ViewModel.Initialise(false);
            ViewModel.FindEpisodesToDownload(null);
            var testException = new Exception("TEST EXCEPTION");
            var syncItemMocker = SetupFireStatusEvent(EPISODE_1_ID, StatusUpdateLevel.Error, false, testException, "test exception");
            Fake.ClearRecordedCalls(MockStatusAndProgressMessageStore);
            Fake.ClearRecordedCalls(MockAnalyticsEngine);

            // act
            ViewModel.DownloadAllPodcastsWithoutNetworkCheck().Wait();

            // assert
            A.CallTo(() => MockCrashReporter.LogNonFatalException(
                A<string>.That.Matches(s => s == "test exception"),
                testException))
                .MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => MockStatusAndProgressMessageStore.StoreMessage(
                A<Guid>.That.Matches(g => g.ToString() == EPISODE_1_ID.ToString()),
                A<string>.That.Matches(s => s == "test exception")))
                .MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => MockStatusAndProgressMessageStore.StoreMessage(
                A<Guid>.That.Matches(g => g.ToString() == EPISODE_1_ID.ToString()),
                A<string>.That.Matches(s => s == "System.Exception: TEST EXCEPTION")))
                .MustHaveHappened(1, Times.Exactly);
            Assert.AreEqual("test exception", ObservedResults.LastUpdateStatusMessage, "message");
            Assert.AreEqual(syncItemMocker.GetMockedSyncItem(), ObservedResults.LastUpdateStatusItem, "item");
            Assert.AreEqual(Status.Error, ObservedResults.LastUpdateStatus, "status");
            A.CallTo(MockAnalyticsEngine).MustNotHaveHappened();
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

        private SyncItemMocker SetupFireStatusEvent(Guid id, StatusUpdateLevel level, bool complete, Exception ex, string message)
        {
            var syncItemMocker = new SyncItemMocker().ApplyId(id).ApplyEpisodeTitle("EpisodeTitle");
            EventHandler<StatusUpdateEventArgs> statusEventHandler = null;
            StatusUpdateEventArgs statusArgs =
                ex != null
                ? new StatusUpdateEventArgs(level, message, ex, complete, syncItemMocker.GetMockedSyncItem())
                : new StatusUpdateEventArgs(level, message, complete, syncItemMocker.GetMockedSyncItem());
            A.CallTo(() =>
                MockSyncItemToEpisodeDownloaderTaskConverter.ConvertItemsToTasks
                (
                    A<IList<ISyncItem>>.Ignored,
                    A<EventHandler<StatusUpdateEventArgs>>.Ignored,
                    A<EventHandler<ProgressEventArgs>>.Ignored
                ))
                .ReturnsLazily((IList<ISyncItem> items, EventHandler<StatusUpdateEventArgs> statusEvent, EventHandler<ProgressEventArgs> progressEvent) =>
                {
                    statusEventHandler = statusEvent;
                    return new IEpisodeDownloader[0];
                });
            A.CallTo(() => MockTaskPool.RunAllTasks(A<int>.Ignored, A<ITask[]>.Ignored))
                .Invokes(() =>
                {
                    statusEventHandler?.Invoke(this, statusArgs);
                });
            return syncItemMocker;
        }


    }
}