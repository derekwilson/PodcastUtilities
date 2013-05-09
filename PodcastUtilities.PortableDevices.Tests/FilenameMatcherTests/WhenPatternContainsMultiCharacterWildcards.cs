using NUnit.Framework;

namespace PodcastUtilities.PortableDevices.Tests.FilenameMatcherTests
{
    public class WhenPatternContainsMultiCharacterWildcards : WhenTestingFilenameMatcher
    {
        protected string Pattern { get; set; }

        protected override void GivenThat()
        {
            base.GivenThat();

            Pattern = "a*.tx*";
        }

        [Test]
        public void ItShouldNotMatchIncorrectFilenames()
        {
            Assert.That(FilenameMatcher.IsMatch("123.456", Pattern), Is.False);
            Assert.That(FilenameMatcher.IsMatch("ac.ty", Pattern), Is.False);
            Assert.That(FilenameMatcher.IsMatch("abbbc.ttxt", Pattern), Is.False);
        }

        [Test]
        public void ItShouldMatchCorrectFilenamesWithSameCase()
        {
            Assert.That(FilenameMatcher.IsMatch("abc.txt", Pattern));
            Assert.That(FilenameMatcher.IsMatch("a.tx", Pattern));
            Assert.That(FilenameMatcher.IsMatch("aaaaaaaaaaaaaaaa.txxxxxxxxxxxxxxxxxxx", Pattern));
        }

        [Test]
        public void ItShouldMatchCorrectFilenamesWithDifferentCase()
        {
            Assert.That(FilenameMatcher.IsMatch("ABC.TXT", Pattern));
            Assert.That(FilenameMatcher.IsMatch("A.TX", Pattern));
            Assert.That(FilenameMatcher.IsMatch("AAAAAAAAAAAAAAAA.TXXXXXXXXXXXXXXXXXXX", Pattern));
        }
    }
}