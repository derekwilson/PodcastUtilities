using NUnit.Framework;

namespace PodcastUtilities.Common.Tests.Feeds.CommandGeneratorTests
{
    public class WhenTestingTheGeneratorWithADownloadFolderToken
        : WhenTestingTheGenerator
    {
        protected override void SetupData()
        {
            base.SetupData();
            _command.Command.Value = "COMMAND {downloadfolder}";
            _command.Arguments.Value = "ARGS {downloadfolder}";
            _command.WorkingDirectory.Value = "CWD {downloadfolder}";
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
            Assert.That(_generatedCommand.Command, Is.EqualTo(@"COMMAND TestFolder"));
        }
        [Test]
        public void ItShouldReturnTheArguments()
        {
            Assert.That(_generatedCommand.Arguments, Is.EqualTo(@"ARGS TestFolder"));
        }
        [Test]
        public void ItShouldReturnTheWorkingDirectory()
        {
            Assert.That(_generatedCommand.WorkingDirectory, Is.EqualTo(@"CWD TestFolder"));
        }
    }
}