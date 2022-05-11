using FakeItEasy;
using NUnit.Framework;
using PodcastUtilities.Common;
using System;

namespace PodcastUtilities.AndroidTests.Tests.ViewModel.Download
{
    [TestFixture]
    public class DownloadViewModel_RequestExit : DownloadViewModelBase
    {
        [Test]
        public void RequestExit_DoesNotPrompts()
        {
            // arrange
            SetupMockControlFileFor2Podcasts();
            SetupEpisodesFor2Podcasts();
            ViewModel.Initialise();
            ViewModel.FindEpisodesToDownload();

            // act
            var result = ViewModel.RequestExit();

            // assert
            A.CallTo(() => MockCrashReporter.LogNonFatalException(A<Exception>.Ignored)).MustNotHaveHappened();
            Assert.AreEqual(result, true, "result");
        }

        [Test]
        public void RequestExit_Prompts()
        {
            // arrange
            bool result = true;
            int taskStartCount = 0;
            SetupMockControlFileFor2Podcasts();
            SetupEpisodesFor2Podcasts();
            ViewModel.Initialise();
            ViewModel.FindEpisodesToDownload();

            // act
            A.CallTo(() => MockTaskPool.RunAllTasks(A<int>.Ignored, A<ITask[]>.Ignored))
                .Invokes(() =>
                {
                    taskStartCount++;
                    if (taskStartCount > 1)
                    {
                        throw new Exception("Concurrent calls are not being trapped");
                    }
                    result = ViewModel.RequestExit();
                });
            ViewModel.DownloadAllPodcastsWithoutNetworkCheck().Wait();

            // assert
            A.CallTo(() => MockCrashReporter.LogNonFatalException(A<Exception>.Ignored)).MustNotHaveHappened();
            Assert.AreEqual(result, false, "result");
            Assert.AreEqual("dialog title", ObservedResults.LastExitPromptTitle, "exit prompt title");
            Assert.AreEqual("exit message", ObservedResults.LastExitPromptMessage, "exit prompt message");
            Assert.AreEqual("exit ok", ObservedResults.LastExitPromptOk, "exit prompt ok");
            Assert.AreEqual("exit cancel", ObservedResults.LastExitPromptCancel, "exit prompt cancel");
        }
    }
}