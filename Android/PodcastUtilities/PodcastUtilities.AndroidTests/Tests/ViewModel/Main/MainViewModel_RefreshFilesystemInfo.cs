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
    }
}