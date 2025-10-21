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
            ViewModel.Initialise(false, false, null, false);

            // act
            await ViewModel.DownloadAllPodcastsWithoutNetworkCheck().ConfigureAwait(false);

            // assert
            Assert.AreEqual("No downloads", ObservedResults.LastDisplayMessage);
        }

        [Test]
        public async Task DownloadAllPodcastsWithoutNetworkCheck_StartsAndStops()
        {
            // arrange
            SetupMockControlFileFor2Podcasts();
            SetupEpisodesFor2Podcasts();
            ViewModel.Initialise(false, false, null, false);
            ViewModel.ConnectServiceForUnitTests(MockDownloaderService);
            await ViewModel.DiscoverStartModeFromService()!.ConfigureAwait(false);   // will trigger a call to FindEpisodesToDownload
            A.CallTo(() => MockDownloaderService.StartDownloads(A<List<DownloadRecyclerItem>>.Ignored))
                  .Invokes(() =>
                  {
                      Events.CompleteEvent?.Invoke(this, EventArgs.Empty);
                  });

            // act
            await ViewModel.DownloadAllPodcastsWithoutNetworkCheck().ConfigureAwait(false);

            // assert
            Assert.AreEqual(1, ObservedResults.StartDownloadingCount, "start count");
            Assert.AreEqual(1, ObservedResults.EndDownloadingCount, "end count");
        }

        [Test]
        public async void DownloadAllPodcastsWithoutNetworkCheck_HandlesMultipleCalls_Sequential()
        {
            // arrange
            SetupMockControlFileFor2Podcasts();
            SetupEpisodesFor2Podcasts();
            ViewModel.Initialise(false, false, null, false);
            ViewModel.ConnectServiceForUnitTests(MockDownloaderService);
            await ViewModel.DiscoverStartModeFromService()!.ConfigureAwait(false);   // will trigger a call to FindEpisodesToDownload

            // act
            ViewModel.DownloadAllPodcastsWithoutNetworkCheck().Wait();
            ViewModel.DownloadAllPodcastsWithoutNetworkCheck().Wait();

            // assert
            A.CallTo(() => MockLogger.Warning(A<ILogger.MessageGenerator>.Ignored)).MustNotHaveHappened();
            Assert.AreEqual(2, ObservedResults.StartDownloadingCount, "start count");
            A.CallTo(() => MockDownloadServiceController.StartService()).MustHaveHappened(2, Times.Exactly);
            A.CallTo(() => MockDownloaderService.StartDownloads(
                A<List<DownloadRecyclerItem>>.That.Matches(items => items == ObservedResults.LastDownloadItems)))
                .MustHaveHappened(2, Times.Exactly);
        }

        [Test]
        public async void ProgressUpdate_UpdatesProgress()
        {
            // arrange
            SetupMockControlFileFor2Podcasts();
            SetupEpisodesFor2Podcasts();
            ViewModel.Initialise(false, false, null, false);
            ViewModel.ConnectServiceForUnitTests(MockDownloaderService);
            await ViewModel.DiscoverStartModeFromService()!.ConfigureAwait(false);   // will trigger a call to FindEpisodesToDownload
            var syncItemMocker = SetupFireProgressEvent(EPISODE_1_ID, 123);

            // act
            ViewModel.DownloadAllPodcastsWithoutNetworkCheck().Wait();

            // assert
            Assert.AreEqual(123, ObservedResults.LastUpdatePercentage, "percentage");
            Assert.AreEqual(syncItemMocker.GetMockedSyncItem(), ObservedResults.LastUpdatePercentageItem, "item");
        }

        [Test]
        public async void StatusUpdate_StatusMessage()
        {
            // arrange
            SetupMockControlFileFor2Podcasts();
            SetupEpisodesFor2Podcasts();
            ViewModel.Initialise(false, false, null, false);
            ViewModel.ConnectServiceForUnitTests(MockDownloaderService);
            await ViewModel.DiscoverStartModeFromService()!.ConfigureAwait(false);   // will trigger a call to FindEpisodesToDownload
            var syncItemMocker = SetupFireStatusEvent(EPISODE_1_ID, Status.Error, "test message");

            // act
            ViewModel.DownloadAllPodcastsWithoutNetworkCheck().Wait();

            // assert
            Assert.AreEqual("test message", ObservedResults.LastUpdateStatusMessage, "message");
            Assert.AreEqual(syncItemMocker.GetMockedSyncItem(), ObservedResults.LastUpdateStatusItem, "item");
            Assert.AreEqual(Status.Error, ObservedResults.LastUpdateStatus, "status");
        }

        [Test]
        public async void ExceptionUpdate()
        {
            // arrange
            SetupMockControlFileFor2Podcasts();
            SetupEpisodesFor2Podcasts();
            ViewModel.Initialise(false, false, null, false);
            ViewModel.ConnectServiceForUnitTests(MockDownloaderService);
            await ViewModel.DiscoverStartModeFromService()!.ConfigureAwait(false);   // will trigger a call to FindEpisodesToDownload
            A.CallTo(() => MockDownloaderService.StartDownloads(A<List<DownloadRecyclerItem>>.Ignored))
                  .Invokes(() =>
                  {
                      Events.ExceptionEvent?.Invoke(this, EventArgs.Empty);
                  });

            // act
            ViewModel.DownloadAllPodcastsWithoutNetworkCheck().Wait();

            // assert
            Assert.AreEqual(1, ObservedResults.DisplayErrorMessageCount, "display error message count");
        }

        private SyncItemMocker SetupFireProgressEvent(Guid id, int percentage)
        {
            var syncItemMocker = new SyncItemMocker().ApplyId(id).ApplyEpisodeTitle("EpisodeTitle");
            A.CallTo(() => MockDownloaderService.StartDownloads(A<List<DownloadRecyclerItem>>.Ignored))
                .Invokes(() =>
                {
                    Events.UpdateItemProgressEvent?.Invoke(this, Tuple.Create(syncItemMocker.GetMockedSyncItem(), percentage));
                });

            return syncItemMocker;
        }

        private SyncItemMocker SetupFireStatusEvent(Guid id, Status status, string message)
        {
            var syncItemMocker = new SyncItemMocker().ApplyId(id).ApplyEpisodeTitle("EpisodeTitle");
            A.CallTo(() => MockDownloaderService.StartDownloads(A<List<DownloadRecyclerItem>>.Ignored))
                .Invokes(() =>
                {
                    Events.UpdateItemStatusEvent?.Invoke(this, Tuple.Create(syncItemMocker.GetMockedSyncItem(), status, message));
                });

            return syncItemMocker;
        }


    }
}