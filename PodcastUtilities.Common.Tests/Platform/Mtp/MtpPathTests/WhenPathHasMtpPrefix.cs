using NUnit.Framework;
using PodcastUtilities.Common.Platform.Mtp;

namespace PodcastUtilities.Common.Tests.Platform.Mtp.MtpPathTests
{
    public class WhenPathHasMtpPrefix
        : WhenTestingBehaviour
    {
        private string _pathToTest;

        protected override void GivenThat()
        {
            base.GivenThat();

            _pathToTest = @"mtp:\my device\path";
        }

        protected override void When()
        {
        }

        [Test]
        public void IsMtpPathShouldReturnTrue()
        {
            Assert.That(MtpPath.IsMtpPath(_pathToTest), Is.True);
        }

        [Test]
        public void StripMtpPrefixShouldRemoveMtpPrefix()
        {
            Assert.That(MtpPath.StripMtpPrefix(_pathToTest), Is.EqualTo(@"my device\path"));
        }

        [Test]
        public void MakeFullPathShouldReturnOriginalPath()
        {
            Assert.That(MtpPath.MakeFullPath(_pathToTest), Is.EqualTo(@"mtp:\my device\path"));
        }

        [Test]
        public void GetMtpPathInfoShouldSetIsMtpPathToTrue()
        {
            Assert.That(MtpPath.GetPathInfo(_pathToTest).IsMtpPath, Is.True);
        }

        [Test]
        public void GetMtpPathInfoShouldCorrectlySetDeviceName()
        {
            Assert.That(MtpPath.GetPathInfo(_pathToTest).DeviceName, Is.EqualTo("my device"));
        }

        [Test]
        public void GetMtpPathInfoShouldCorrectlySetPath()
        {
            Assert.That(MtpPath.GetPathInfo(_pathToTest).RelativePathOnDevice, Is.EqualTo("path"));
        }
    }
}