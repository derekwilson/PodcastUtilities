using NUnit.Framework;

namespace PodcastUtilities.Common.Tests.Feeds.CommandGeneratorTests
{
    public class WhenTestingTheGeneratorWithANullCommandline
        : WhenTestingTheGenerator
    {
        /// <summary>
        /// Invoke the action being tested.
        /// </summary>
        protected override void When()
        {
            GeneratedCommand = Generator.ReplaceTokensInCommandline(null, null, null, null);
        }

        [Test]
        public void ItShouldReturnNull()
        {
            Assert.That(GeneratedCommand, Is.Null);
        }
    }
}
