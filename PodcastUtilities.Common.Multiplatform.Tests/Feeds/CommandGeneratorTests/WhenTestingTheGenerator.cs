#region License
// FreeBSD License
// Copyright (c) 2010 - 2013, Andrew Trevarrow and Derek Wilson
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
// 
// Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
// 
// Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED 
// WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A 
// PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR
// ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED
// TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING 
// NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
// POSSIBILITY OF SUCH DAMAGE.
#endregion
using System;
using Moq;
using NUnit.Framework;
using PodcastUtilities.Common.Configuration;
using PodcastUtilities.Common.Feeds;
using PodcastUtilities.Common.Platform;

namespace PodcastUtilities.Common.Multiplatform.Tests.Feeds.CommandGeneratorTests
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

        protected Mock<IEnvironmentInformationProvider> _environmentInformationProvider;
        protected Mock<IDirectoryInfo> _directoryInfo;

        protected override void GivenThat()
        {
            base.GivenThat();

            _controlFile = TestControlFileFactory.CreateControlFile();

            _environmentInformationProvider = GenerateMock<IEnvironmentInformationProvider>();
            _directoryInfo = GenerateMock<IDirectoryInfo>();

            SetupData();
            SetupStubs();

            _generator = new CommandGenerator(_environmentInformationProvider.Object);
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
            _directoryInfo.Setup(di => di.FullName).Returns(@"c:\test");
            _environmentInformationProvider.Setup(ei => ei.GetCurrentApplicationDirectory()).Returns(_directoryInfo.Object);
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