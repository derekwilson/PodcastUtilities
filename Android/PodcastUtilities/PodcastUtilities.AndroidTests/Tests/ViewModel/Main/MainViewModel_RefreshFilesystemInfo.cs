using FakeItEasy;
using NUnit.Framework;

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
            A.CallToSet(() => MockDriveVolumeInfoView.Title).To(() => PATH1).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => MockDriveVolumeInfoView.SetSpace(100, 200, "100", "MB", "200", "MB")).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => MockDriveVolumeInfoView.GetView()).MustHaveHappened(1, Times.Exactly);
        }
    }
}