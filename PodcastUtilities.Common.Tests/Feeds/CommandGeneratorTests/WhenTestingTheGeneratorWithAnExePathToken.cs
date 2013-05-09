using NUnit.Framework;

namespace PodcastUtilities.Common.Tests.Feeds.CommandGeneratorTests
{
    public class WhenTestingTheGeneratorWithAnExePathToken
        : WhenTestingTheGenerator
    {
        protected override void SetupData()
        {
            base.SetupData();
            _command.Command.Value = "COMMAND {exefolder}";
            _command.Arguments.Value = "ARGS {exefolder}";
            _command.WorkingDirectory.Value = "CWD {exefolder}";
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
            Assert.That(_generatedCommand.Command, Is.EqualTo(@"COMMAND c:\test"));
        }
        [Test]
        public void ItShouldReturnTheArguments()
        {
            Assert.That(_generatedCommand.Arguments, Is.EqualTo(@"ARGS c:\test"));
        }
        [Test]
        public void ItShouldReturnTheWorkingDirectory()
        {
            Assert.That(_generatedCommand.WorkingDirectory, Is.EqualTo(@"CWD c:\test"));
        }
    }
}