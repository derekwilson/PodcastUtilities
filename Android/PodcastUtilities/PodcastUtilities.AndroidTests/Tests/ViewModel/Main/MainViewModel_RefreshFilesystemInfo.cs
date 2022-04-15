using FakeItEasy;
using NUnit.Framework;
using System;

namespace PodcastUtilities.AndroidTests.Tests.ViewModel.Main
{
    [TestFixture]
    public class MainViewModel_RefreshFilesystemInfo : MainViewModelBase
    {
        [Test]
        public void RefreshFilesystemInfo_Sets_TheNodriveMessage()
        {
            // arrange
            ViewModel.Initialise();

            // act
            ViewModel.RefreshFileSystemInfo();

            // assert
            Assert.AreEqual(1, ObservedResults.ShowNoDriveMessageCount);
        }

        [Test]
        public void RefreshFilesystemInfo_Adds_DriveInfo()
        {
            // arrange
            SetupFileSystem();
            ViewModel.Initialise();

            // act
            ViewModel.RefreshFileSystemInfo();

            // assert
            // there are no errors
            A.CallTo(() => MockCrashReporter.LogNonFatalException(A<Exception>.Ignored)).MustNotHaveHappened();

            // file 1
            A.CallToSet(() => MockDriveVolumeInfoView.Title).To(() => PATH1).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => MockDriveVolumeInfoView.SetSpace(100, 200, "100", "MB", "200", "MB")).MustHaveHappened(1, Times.Exactly);

            // file 2
            A.CallToSet(() => MockDriveVolumeInfoView.Title).To(() => PATH2).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => MockDriveVolumeInfoView.SetSpace(2000, 4000, "1.95", "GB", "3.91", "GB")).MustHaveHappened(1, Times.Exactly);

            // both
            A.CallTo(() => MockDriveVolumeInfoView.GetView()).MustHaveHappened(2, Times.Exactly);
        }
    }
}