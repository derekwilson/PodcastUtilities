using System;
using NUnit.Framework;
using PodcastUtilities.Common.Configuration;
using PodcastUtilities.Common.Feeds;

namespace PodcastUtilities.Common.Tests.Feeds.CommandGeneratorTests
{
    public abstract class WhenTestingTheGenerator
        : WhenTestingBehaviour
    {
        protected CommandGenerator _generator;

        protected ITokenisedCommand _command;

        protected IExternalCommand _generatedCommand;

        protected string _rootFolder;
        protected string _destinationPathname;
        protected string _feedAddress;
        protected FeedInfo _feedInfo;
        protected PodcastInfo _podcastInfo;
        protected IReadOnlyControlFile _controlFile;

        protected override void GivenThat()
        {
            base.GivenThat();

            _controlFile = TestControlFileFactory.CreateControlFile();

            SetupData();
            SetupStubs();

            _generator = new CommandGenerator();
        }

        protected virtual void SetupData()
        {
            _command = new TokenisedCommand(_controlFile);
            _command.Command.Value = "COMMAND";
            _command.Arguments.Value = "ARGS";
            _command.WorkingDirectory.Value = "CWD";

            _rootFolder = @"c:\root";
            _destinationPathname = @"c:\root\TestFolder\destination.xyz";
            _podcastInfo = new PodcastInfo(_controlFile);

            _feedAddress = "http://test";

            _feedInfo = new FeedInfo(_controlFile);
            _feedInfo.Format.Value = PodcastFeedFormat.RSS;
            _feedInfo.NamingStyle.Value = PodcastEpisodeNamingStyle.UrlFileName;
            _feedInfo.Address = new Uri(_feedAddress);
            _feedInfo.MaximumDaysOld.Value = int.MaxValue;
            _feedInfo.DownloadStrategy.Value = PodcastEpisodeDownloadStrategy.All;

            _podcastInfo = new PodcastInfo(_controlFile);
            _podcastInfo.Folder = "TestFolder";
            _podcastInfo.Feed = _feedInfo;
        }

        protected virtual void SetupStubs()
        {
        }
    }

    public class WhenTestingTheGeneratorWithoutTokens : WhenTestingTheGenerator
    {
        /// <summary>
        /// Invoke the action being tested.
        /// </summary>
        protected override void When()
        {
            _generatedCommand = _generator.ReplaceTokensInCommand(_command, _rootFolder, _destinationPathname, _podcastInfo);
        }

        [Test]
        public void ItShouldReturnNull()
        {
            Assert.That(_generatedCommand.Command, Is.EqualTo("COMMAND"));
        }
    }
}