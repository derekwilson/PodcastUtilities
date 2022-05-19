using FakeItEasy;
using NUnit.Framework;
using PodcastUtilities.AndroidLogic.Logging;
using PodcastUtilities.Common.Platform;
using System;
using System.Collections.Generic;

namespace PodcastUtilities.AndroidTests.Tests.ViewModel.Purge
{
    [TestFixture]
    public class PurgeViewModel_PurgeAllItems : PurgeViewModelBase
    {
        protected const string FILE_PATH = "/sdcard/file.ext";
        protected const string DIR_PATH = "/sdcard/dir";

        [Test]
        public void PurgeAllItems_DeletesItems()
        {
            // arrange
            SetupMockControlFileFor2Podcasts();
            var dir = SetupMockPurgerWithRealItems();
            ViewModel.Initialise();
            ViewModel.FindItemsToDelete();

            // act
            ViewModel.PurgeAllItems();

            // assert
            A.CallTo(() => MockFileUtilities.FileDelete(FILE_PATH)).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => MockEpisodePurger.PurgeFolder(dir)).MustHaveHappened(1, Times.Exactly);
        }

        [Test]
        public void PurgeAllItems_RespectsSelectionFile()
        {
            // arrange
            SetupMockControlFileFor2Podcasts();
            var dir = SetupMockPurgerWithRealItems();
            ViewModel.Initialise();
            ViewModel.FindItemsToDelete();
            ObservedResults.LastPurgeItems[0].Selected = false;

            // act
            ViewModel.PurgeAllItems();

            // assert
            A.CallTo(MockFileUtilities).MustNotHaveHappened();
            A.CallTo(() => MockEpisodePurger.PurgeFolder(dir)).MustHaveHappened(1, Times.Exactly);
        }

        [Test]
        public void PurgeAllItems_RespectsSelectionDir()
        {
            // arrange
            SetupMockControlFileFor2Podcasts();
            var dir = SetupMockPurgerWithRealItems();
            ViewModel.Initialise();
            ViewModel.FindItemsToDelete();
            ObservedResults.LastPurgeItems[1].Selected = false;

            // act
            ViewModel.PurgeAllItems();

            // assert
            A.CallTo(() => MockFileUtilities.FileDelete(FILE_PATH)).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => MockEpisodePurger.PurgeFolder(A<IDirectoryInfo>.Ignored)).MustNotHaveHappened();
        }

        [Test]
        public void PurgeAllItems_HandlesException()
        {
            // arrange
            SetupMockControlFileFor2Podcasts();
            var dir = SetupMockPurgerWithRealItems();
            ViewModel.Initialise();
            ViewModel.FindItemsToDelete();
            var testException = new Exception("TEST EXCEPTION");
            A.CallTo(MockFileUtilities).Throws(testException);

            // act
            ViewModel.PurgeAllItems();

            // assert
            A.CallTo(() => MockCrashReporter.LogNonFatalException(testException)).MustHaveHappened(1, Times.Exactly);
            Assert.AreEqual("Error file.ext", ObservedResults.LastDisplayMessage);
        }

        private IDirectoryInfo SetupMockPurgerWithRealItems()
        {
            SystemFileInfoProvider fileProvider = new SystemFileInfoProvider();
            A.CallTo(() => MockEpisodePurger.FindEpisodesToPurge(SOURCE_ROOT, podcast1Mocker.GetMockedPodcastInfo()))
                .Returns(
                    new List<IFileInfo>()
                    {
                        fileProvider.GetFileInfo(FILE_PATH)
                    }
                );
            SystemDirectoryInfoProvider dirProvider = new SystemDirectoryInfoProvider();
            var dir = dirProvider.GetDirectoryInfo(DIR_PATH);
            A.CallTo(() => MockEpisodePurger.FindEmptyFoldersToDelete(SOURCE_ROOT, podcast1Mocker.GetMockedPodcastInfo(), A<IList<IFileInfo>>.Ignored))
                .Returns(
                    new List<IDirectoryInfo>()
                    {
                        dir
                    }
                );
            return dir;
        }
    }
}