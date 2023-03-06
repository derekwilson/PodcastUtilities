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
using System.Collections.Generic;
using System.IO;
using Moq;
using PodcastUtilities.Common.Configuration;
using PodcastUtilities.Common.Feeds;
using PodcastUtilities.Common.Platform;

namespace PodcastUtilities.Common.Multiplatform.Tests.Feeds.EpisodeFinderTests
{
    public abstract class WhenUsingTheEpisodeFinder
        : WhenTestingBehaviour
    {
        protected EpisodeFinder _episodeFinder;

        protected bool _retainFeedXml;

        protected Mock<IWebClientFactory> _webClientFactory;
        protected Mock<IWebClient> _webClient;
        protected Mock<IPodcastFeedFactory> _feedFactory;
        protected Mock<IFileUtilities> _fileUtilities;
        protected Mock<ITimeProvider> _timeProvider;
        protected Mock<IStateProvider> _stateProvider;
        protected Mock<IState> _state;
        protected Mock<IDirectoryInfoProvider> _directoryInfoProvider;
        protected Mock<IDirectoryInfo> _directoryInfo;
        protected Mock<ICommandGenerator> _commandGenerator;
        protected Mock<IPathUtilities> _pathUtilities;
        protected Mock<IPodcastFeed> _podcastFeed;

        protected string _rootFolder;
        protected int _retryWaitTime;
        protected PodcastInfo _podcastInfo;
        protected FeedInfo _feedInfo;
        protected IList<ISyncItem> _episodesToSync;
        protected string _feedAddress;
        protected MemoryStream _stream;
        protected IList<IPodcastFeedItem> _podcastFeedItems;
        protected IExternalCommand _externalCommand;

        protected DateTime _now;

        protected StatusUpdateEventArgs _latestUpdate;

        protected IReadOnlyControlFile _controlFile;

        protected override void GivenThat()
        {
            base.GivenThat();

            _controlFile = TestControlFileFactory.CreateControlFile();

            _stateProvider = GenerateMock<IStateProvider>();
            _state = GenerateMock<IState>();
            _timeProvider = GenerateMock<ITimeProvider>();
            _webClientFactory = GenerateMock<IWebClientFactory>();
            _webClient = GenerateMock<IWebClient>();
            _feedFactory = GenerateMock<IPodcastFeedFactory>();
            _fileUtilities = GenerateMock<IFileUtilities>();
            _podcastFeed = GenerateMock<IPodcastFeed>();
            _directoryInfoProvider = GenerateMock<IDirectoryInfoProvider>();
            _directoryInfo = GenerateMock<IDirectoryInfo>();
            _commandGenerator = GenerateMock<ICommandGenerator>();
            _pathUtilities = GenerateMock<IPathUtilities>();

            SetupData();
            SetupStubs();

            _episodeFinder = new EpisodeFinder(
                _fileUtilities.Object, 
                _feedFactory.Object,
                _webClientFactory.Object,
                _timeProvider.Object,
                _stateProvider.Object,
                _directoryInfoProvider.Object,
                _commandGenerator.Object,
                _pathUtilities.Object);
            _episodeFinder.StatusUpdate += new EventHandler<StatusUpdateEventArgs>(EpisodeFinderStatusUpdate);
            _latestUpdate = null;
        }

        void EpisodeFinderStatusUpdate(object sender, StatusUpdateEventArgs e)
        {
            _latestUpdate = e;
        }

        protected virtual void SetupData()
        {
            _retainFeedXml = false;

            _now = new DateTime(2010, 5, 1, 16, 11, 12);

            _feedAddress = "http://test";

            _feedInfo = new FeedInfo(_controlFile);
            _feedInfo.Format.Value = PodcastFeedFormat.RSS;
            _feedInfo.NamingStyle.Value = PodcastEpisodeNamingStyle.UrlFileName;
            _feedInfo.Address = new Uri(_feedAddress);
            _feedInfo.MaximumDaysOld.Value = int.MaxValue;
            _feedInfo.DownloadStrategy.Value = PodcastEpisodeDownloadStrategy.All;

            _retryWaitTime = 13;
            _rootFolder = "c:\\TestRoot";
            _podcastInfo = new PodcastInfo(_controlFile);
            _podcastInfo.Folder = "TestFolder";
            _podcastInfo.Feed = _feedInfo;

            _podcastFeedItems = new List<IPodcastFeedItem>(10);

            _externalCommand = new ExternalCommand();
        }

        protected virtual void SetupStubs()
        {
            SetupStubs(false);
        }

        protected virtual void SetupStubs(bool throwErrorFromFeed)
        {
            _timeProvider.Setup(time => time.UtcNow).Returns(_now);
            _webClient.Setup(client => client.OpenRead(_feedInfo.Address)).Returns(_stream);
            _webClientFactory.Setup(factory => factory.CreateWebClient()).Returns(_webClient.Object);
            _feedFactory.Setup(factory => factory.CreatePodcastFeed(_feedInfo.Format.Value, _stream, null)).Returns(_podcastFeed.Object);
            if (throwErrorFromFeed)
            {
                _podcastFeed.Setup(feed => feed.Episodes).Throws(new Exception("ERROR"));
            }
            else
            {
                _podcastFeed.Setup(feed => feed.Episodes).Returns(_podcastFeedItems);
            }
            _stateProvider.Setup(provider => provider.GetState(Path.Combine(_rootFolder, _podcastInfo.Folder))).Returns(_state.Object);
            _directoryInfoProvider.Setup(dir => dir.GetDirectoryInfo(Path.Combine(_rootFolder, _podcastInfo.Folder))).Returns(_directoryInfo.Object);

            _commandGenerator.Setup(cmd => cmd.ReplaceTokensInCommand(
                It.IsAny<ITokenisedCommand>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<IPodcastInfo>()))
                .Returns(_externalCommand);
            _pathUtilities.Setup(utilities => utilities.GetPathSeparator()).Returns('\\');
        }
    }
}
