using NUnit.Framework;

namespace PodcastUtilities.PortableDevices.Tests.FilenameMatcherTests
{
    public class WhenPatternDoesNotContainWildcards : WhenTestingFilenameMatcher
    {
        [Test]
        public void ItShouldNotMatchFilenameDifferentToPattern()
        {
            Assert.That(FilenameMatcher.IsMatch("123.456", "abc.xyz"), Is.False);
        }

        [Test]
        public void ItShouldMatchFilenameSameAsPattern()
        {
            Assert.That(FilenameMatcher.IsMatch("abc.xyz", "abc.xyz"));
        }

        [Test]
        public void ItShouldMatchFilenameSameAsPatternButDifferentCase()
        {
            Assert.That(FilenameMatcher.IsMatch("aBc.XyZ", "abc.xyz"));
        }
    }
}