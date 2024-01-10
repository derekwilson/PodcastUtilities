using FakeItEasy;
using NUnit.Framework;
using PodcastUtilities.Common;
using System;

namespace PodcastUtilities.AndroidTests.Tests.ViewModel.Download
{
    [TestFixture]
    public class DownloadViewModel_CancelAllJobsAndExit : DownloadViewModelBase
    {
        [Test]
        public void CancelAllJobsAndExit_CancelsJobs()
        {
            // arrange
            SetupMockControlFileFor2Podcasts();
            SetupEpisodesFor2Podcasts();
            ViewModel.Initialise(false);
            ViewModel.FindEpisodesToDownload(null);
            ViewModel.RequestExit();

            // act
            ViewModel.CancelAllJobsAndExit();

            // assert
            A.CallTo(() => MockCrashReporter.LogNonFatalException(A<Exception>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => MockTaskPool.CancelAllTasks()).MustHaveHappened(1, Times.Exactly);
            Assert.AreEqual("cancelling", ObservedResults.LastDisplayMessage);
        }

        [Test]
        public void CancelAllJobsAndExit_Exits()
        {
            // arrange
            int taskStartCount = 0;
            SetupMockControlFileFor2Podcasts();
            SetupEpisodesFor2Podcasts();
            ViewModel.Initialise(false);
            ViewModel.FindEpisodesToDownload(null);

            // act
            A.CallTo(() => MockTaskPool.RunAllTasks(A<int>.Ignored, A<ITask[]>.Ignored))
                .Invokes(() =>
                {
                    taskStartCount++;
                    if (taskStartCount > 1)
                    {
                        throw new Exception("Concurrent calls are not being trapped");
                    }
                    ViewModel.RequestExit();
                    ViewModel.CancelAllJobsAndExit();
                });
            ViewModel.DownloadAllPodcastsWithoutNetworkCheck().Wait();

            // assert
            A.CallTo(() => MockCrashReporter.LogNonFatalException(A<Exception>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => MockTaskPool.CancelAllTasks()).MustHaveHappened(1, Times.Exactly);
            Assert.AreEqual(1, ObservedResults.ExitCount, "exit count");
        }
    }
}