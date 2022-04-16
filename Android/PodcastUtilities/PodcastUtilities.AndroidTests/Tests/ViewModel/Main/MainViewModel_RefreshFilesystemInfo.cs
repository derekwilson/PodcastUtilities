using FakeItEasy;
using NUnit.Framework;
using PodcastUtilities.AndroidLogic.Logging;
using System;

namespace PodcastUtilities.AndroidTests.Tests.ViewModel.Main
{
    [TestFixture]
    public class MainViewModel_RefreshFilesystemInfo : MainViewModelBase
    {
        [Test]
        public void RefreshFilesystemInfo_Handles_Errors()
        {
            // arrange
            SetupFileSystem();
            ViewModel.Initialise();
            var testException = new Exception();
            A.CallTo(MockFileSystemHelper).Throws(testException);

            // act
            ViewModel.RefreshFileSystemInfo();

            // assert
            // writes the error to the logs
            A.CallTo(() => MockLogger.LogException(A<ILogger.MessageGenerator>.Ignored,testException)).MustHaveHappened(1, Times.Exactly);
            // writes the error to crshlytics
            A.CallTo(() => MockCrashReporter.LogNonFatalException(A<Exception>.Ignored)).MustHaveHappened(1, Times.Exactly);
        }

        [Test]
        public void RefreshFilesystemInfo_Sets_TheNodriveMessage()
        {
            // arrange
            SetupFileSystem();
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

        [Test]
        public void RefreshFilesystemInfo_Adds_DriveInfo_Before_An_Error()
        {
            // arrange
            SetupFileSystem();
            ViewModel.Initialise();
            // throw processing the second item
            var testException = new Exception();
            A.CallTo(() => MockFileSystemHelper.GetAvailableFileSystemSizeInBytes(PATH2)).Throws(testException);

            // act
            ViewModel.RefreshFileSystemInfo();

            // assert
            // writes the error to the logs
            A.CallTo(() => MockLogger.LogException(A<ILogger.MessageGenerator>.Ignored, testException)).MustHaveHappened(1, Times.Exactly);
            // writes the error to crshlytics
            A.CallTo(() => MockCrashReporter.LogNonFatalException(A<Exception>.Ignored)).MustHaveHappened(1, Times.Exactly);

            // file 1
            A.CallToSet(() => MockDriveVolumeInfoView.Title).To(() => PATH1).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => MockDriveVolumeInfoView.SetSpace(100, 200, "100", "MB", "200", "MB")).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => MockDriveVolumeInfoView.GetView()).MustHaveHappened(1, Times.Exactly);
        }

    }
}