using NUnit.Framework;

namespace PodcastUtilities.Common.Tests.Feeds.CommandGeneratorTests
{
    public class WhenTestingTheGeneratorWithADestinationPathToken
        : WhenTestingTheGenerator
    {
        protected override void SetupData()
        {
            base.SetupData();
            _command.Command.Value = "COMMAND {downloadfullpath}";
            _command.Arguments.Value = "ARGS {downloadfullpath}";
            _command.WorkingDirectory.Value = "CWD {downloadfullpath}";
        }

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
            Assert.That(_generatedCommand.Command, Is.EqualTo(@"COMMAND c:\root\TestFolder\destination.xyz"));
        }
        [Test]
        public void ItShouldReturnTheArguments()
        {
            Assert.That(_generatedCommand.Arguments, Is.EqualTo(@"ARGS c:\root\TestFolder\destination.xyz"));
        }
        [Test]
        public void ItShouldReturnTheWorkingDirectory()
        {
            Assert.That(_generatedCommand.WorkingDirectory, Is.EqualTo(@"CWD c:\root\TestFolder\destination.xyz"));
        }
    }
}