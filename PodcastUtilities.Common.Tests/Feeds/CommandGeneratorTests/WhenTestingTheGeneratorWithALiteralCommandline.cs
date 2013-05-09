using NUnit.Framework;

namespace PodcastUtilities.Common.Tests.Feeds.CommandGeneratorTests
{
    public class WhenTestingTheGeneratorWithALiteralCommandline
        : WhenTestingTheGenerator
    {
        /// <summary>
        /// Invoke the action being tested.
        /// </summary>
        protected override void When()
        {
            _generatedCommand = _generator.ReplaceTokensInCommand(_command, _rootFolder, _destinationPathname, _podcastInfo);
        }

        [Test]
        public void ItShouldReturnTheCommand()
        {
            Assert.That(_generatedCommand.Command, Is.EqualTo("COMMAND"));
        }
        [Test]
        public void ItShouldReturnTheArguments()
        {
            Assert.That(_generatedCommand.Arguments, Is.EqualTo("ARGS"));
        }
        [Test]
        public void ItShouldReturnTheWorkingDirectory()
        {
            Assert.That(_generatedCommand.WorkingDirectory, Is.EqualTo("CWD"));
        }
    }
}