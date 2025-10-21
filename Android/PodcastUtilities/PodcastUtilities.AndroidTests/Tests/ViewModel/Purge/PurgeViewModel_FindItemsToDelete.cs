using FakeItEasy;
using NUnit.Framework;
using PodcastUtilities.AndroidLogic.Logging;
using PodcastUtilities.Common.Platform;

namespace PodcastUtilities.AndroidTests.Tests.ViewModel.Purge
{
    [TestFixture]
    public class PurgeViewModel_FindItemsToDelete : PurgeViewModelBase
    {
        [Test]
        public void FindItemsToDelete_HandlesNoControlFile()
        {
            // arrange
            A.CallTo(() => MockApplicationControlFileProvider.GetApplicationConfiguration()).Returns(null!);
            ViewModel.Initialise();

            // act
            ViewModel.FindItemsToDelete();

            // assert
            A.CallTo(() => MockLogger.Warning(A<ILogger.MessageGenerator>.Ignored)).MustHaveHappened(1, Times.Exactly);
        }

        [Test]
        public void FindItemsToDelete_UpdatesProgress()
        {
            // arrange
            SetupMockControlFileFor2Podcasts();
            ViewModel.Initialise();

            // act
            ViewModel.FindItemsToDelete();

            // assert
            Assert.AreEqual(4, ObservedResults.StartProgress?[0], "total number of feeds * 2");
            Assert.AreEqual(1, ObservedResults.UpdateProgress?[0], "updated to 1");
            Assert.AreEqual(2, ObservedResults.UpdateProgress?[1], "updated to 2");
            Assert.AreEqual(3, ObservedResults.UpdateProgress?[2], "updated to 3");
            Assert.AreEqual(4, ObservedResults.UpdateProgress?[3], "updated to 4");
            Assert.AreEqual(1, ObservedResults.EndProgressCount, "ended once");
        }

        [Test]
        public void FindItemsToDelete_Adds()
        {
            // arrange
            SetupMockControlFileFor2Podcasts();
            SetupMockPurger();
            ViewModel.Initialise();

            // act
            ViewModel.FindItemsToDelete();

            // assert
            Assert.AreEqual(4, ObservedResults.LastPurgeItems?.Count, "total number items found");
            Assert.AreEqual("File1", GetDisplayName(ObservedResults.LastPurgeItems?[0].FileOrDirectoryItem), "item 1 name");
            Assert.AreEqual("File2", GetDisplayName(ObservedResults.LastPurgeItems?[1].FileOrDirectoryItem), "item 2 name");
            Assert.AreEqual("Dir1Full", GetDisplayName(ObservedResults.LastPurgeItems?[2].FileOrDirectoryItem), "item 3 name");
            Assert.AreEqual("Dir2Full", GetDisplayName(ObservedResults.LastPurgeItems?[3].FileOrDirectoryItem), "item 4 name");
        }

        [Test]
        public void FindItemsToDelete_Analytics()
        {
            // arrange
            SetupMockControlFileFor2Podcasts();
            SetupMockPurger();
            ViewModel.Initialise();

            // act
            ViewModel.FindItemsToDelete();

            // assert
            A.CallTo(() => MockAnalyticsEngine.PurgeScanEvent(4)).MustHaveHappened(1, Times.Exactly);
        }

        [Test]
        public void FindItemsToDelete_SetsTitle()
        {
            // arrange
            SetupMockControlFileFor2Podcasts();
            SetupMockPurger();
            ViewModel.Initialise();

            // act
            ViewModel.FindItemsToDelete();

            // assert
            Assert.AreEqual("item count == 4", ObservedResults.LastSetTitle);
        }

        [Test]
        public void FindItemsToDelete_DoesNotRunTwice()
        {
            // arrange
            SetupMockControlFileFor2Podcasts();
            SetupMockPurger();
            ViewModel.Initialise();
            ViewModel.FindItemsToDelete();
            ResetObservedResults();

            // act
            ViewModel.FindItemsToDelete();

            // assert
            // we only use the purger twice - for each episode
            A.CallTo(MockEpisodePurger).MustHaveHappened(4, Times.Exactly);
            Assert.AreEqual(0, ObservedResults.StartProgressCount, "never started");
            Assert.AreEqual(0, ObservedResults.UpdateProgressCount, "never updated");
            Assert.AreEqual(0, ObservedResults.EndProgressCount, "never ended");

            // however we do reinitialise the UI
            Assert.AreEqual(4, ObservedResults.LastPurgeItems?.Count, "total items found");
            Assert.AreEqual("item count == 4", ObservedResults.LastSetTitle);
        }


        private string GetDisplayName(dynamic item)
        {
            if (item is IFileInfo)
            {
                return ((IFileInfo)item).Name;
            }
            if (item is IDirectoryInfo)
            {
                return ((IDirectoryInfo)item).FullName;
            }
            return "UNKNOWN TYPE";
        }
    }
}