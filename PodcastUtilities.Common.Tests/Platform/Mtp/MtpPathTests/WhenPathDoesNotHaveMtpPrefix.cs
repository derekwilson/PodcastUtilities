using NUnit.Framework;
using PodcastUtilities.Common.Platform.Mtp;

namespace PodcastUtilities.Common.Tests.Platform.Mtp.MtpPathTests
{
    public class WhenPathDoesNotHaveMtpPrefix
        : WhenTestingBehaviour
    {
        private string _pathToTest;

        protected override void GivenThat()
        {
            base.GivenThat();

            _pathToTest = @"abc\def\ghi";
        }

        protected override void When()
        {
        }

        [Test]
        public void IsMtpPathShouldReturnFalse()
        {
            Assert.That(MtpPath.IsMtpPath(_pathToTest), Is.False);
        }

        [Test]
        public void StripMtpPrefixShouldReturnOriginalPath()
        {
            Assert.That(MtpPath.StripMtpPrefix(_pathToTest), Is.EqualTo(_pathToTest));
        }

        [Test]
        public void MakeFullPathShouldAddMtpPrefix()
        {
            Assert.That(MtpPath.MakeFullPath(_pathToTest), Is.EqualTo(@"MTP:\abc\def\ghi"));
        }
    }
}