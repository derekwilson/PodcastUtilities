using NUnit.Framework;

namespace PodcastUtilities.PortableDevices.Tests.FilenameMatcherTests
{
    public class WhenPatternContainsSingleCharacterWildcards : WhenTestingFilenameMatcher
    {
        protected string Pattern { get; set; }

        protected override void GivenThat()
        {
            base.GivenThat();

            Pattern = "a?c.x?z";
        }

        [Test]
        public void ItShouldNotMatchIncorrectFilenames()
        {
            Assert.That(FilenameMatcher.IsMatch("123.456", Pattern), Is.False);
            Assert.That(FilenameMatcher.IsMatch("ac.xz", Pattern), Is.False);
            Assert.That(FilenameMatcher.IsMatch("abbbc.xyyyz", Pattern), Is.False);
        }

        [Test]
        public void ItShouldMatchCorrectFilenamesWithSameCase()
        {
            Assert.That(FilenameMatcher.IsMatch("abc.xyz", Pattern));
            Assert.That(FilenameMatcher.IsMatch("a_c.x_z", Pattern));
            Assert.That(FilenameMatcher.IsMatch("a.c.x.z", Pattern));
        }

        [Test]
        public void ItShouldMatchCorrectFilenamesWithDifferentCase()
        {
            Assert.That(FilenameMatcher.IsMatch("ABC.XYZ", Pattern));
            Assert.That(FilenameMatcher.IsMatch("a_C.x_Z", Pattern));
            Assert.That(FilenameMatcher.IsMatch("A1c.X0z", Pattern));
        }
    }
}