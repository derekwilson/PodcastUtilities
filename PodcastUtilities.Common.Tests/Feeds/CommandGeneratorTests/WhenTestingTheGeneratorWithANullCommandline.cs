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
            _generatedCommand = _generator.ReplaceTokensInCommand(null, null, null, null);
        }

        [Test]
        public void ItShouldReturnNull()
        {
            Assert.That(_generatedCommand, Is.Null);
        }
    }
}
