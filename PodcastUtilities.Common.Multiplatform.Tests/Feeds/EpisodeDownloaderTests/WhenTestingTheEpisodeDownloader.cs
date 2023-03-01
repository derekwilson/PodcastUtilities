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
using System.IO;
using Moq;
using PodcastUtilities.Common.Feeds;
using PodcastUtilities.Common.Perfmon;
using PodcastUtilities.Common.Platform;

namespace PodcastUtilities.Common.Multiplatform.Tests.Feeds.EpisodeDownloaderTests
{
    public abstract class WhenTestingTheEpisodeDownloader
        : WhenTestingBehaviour
    {
        protected EpisodeDownloader _downloader;

        protected Mock<IWebClientFactory> _webClientFactory;
        protected Mock<IWebClient> _webClient;

        protected ISyncItem _syncItem;

        protected MemoryStream _stream;

        protected Exception _exception;

        protected StatusUpdateEventArgs _statusUpdateArgs;
        protected ProgressEventArgs _progressUpdateArgs;

        protected Mock<IStateProvider> _stateProvider;
        protected Mock<ICounterFactory> _counterFactory;
        protected Mock<IAverageCounter> _averageCounterTime;
        protected Mock<IAverageCounter> _averageCounterSize;

        protected Mock<ICommandExecuter> _commandExecuter;

        protected Mock<IDirectoryInfoProvider> _directoryInfoProvider;
        protected Mock<IFileUtilities> _fileUtilities;
        protected Mock<IDirectoryInfo> _directoryInfo;
        protected string _downloadFolder;
        protected Mock<IState> _state;
        protected DateTime _published;

        protected override void GivenThat()
        {
            base.GivenThat();

            _webClientFactory = GenerateMock<IWebClientFactory>();
            _webClient = GenerateMock<IWebClient>();
            _directoryInfoProvider = GenerateMock<IDirectoryInfoProvider>();
            _directoryInfo = GenerateMock<IDirectoryInfo>();
            _fileUtilities = GenerateMock<IFileUtilities>();
            _stateProvider = GenerateMock<IStateProvider>();
            _counterFactory = GenerateMock<ICounterFactory>();
            _averageCounterTime = GenerateMock<IAverageCounter>();
            _averageCounterSize = GenerateMock<IAverageCounter>();
            _state = GenerateMock<IState>();
            _commandExecuter = GenerateMock<ICommandExecuter>();

            _syncItem = new SyncItem();
            _exception = null;

            SetupData();
            SetupStubs();

            _downloader = new EpisodeDownloader(_webClientFactory.Object, _directoryInfoProvider.Object, _fileUtilities.Object, _stateProvider.Object, _counterFactory.Object, _commandExecuter.Object);
            _downloader.StatusUpdate += new EventHandler<StatusUpdateEventArgs>(DownloaderStatusUpdate);
            _downloader.ProgressUpdate += new EventHandler<ProgressEventArgs>(DownloaderProgressUpdate);
        }

        protected virtual void DownloaderProgressUpdate(object sender, ProgressEventArgs e)
        {
            _progressUpdateArgs = e;
        }

        protected virtual void DownloaderStatusUpdate(object sender, StatusUpdateEventArgs e)
        {
            _statusUpdateArgs = e;
        }

        protected virtual void SetupData()
        {
            _downloadFolder = "c:\\folder";
            _published = new DateTime(2011, 10, 15, 16, 15, 14);

            _syncItem.EpisodeUrl = new Uri("http://test");
            _syncItem.DestinationPath = Path.Combine(_downloadFolder, "file.ext");
            _syncItem.EpisodeTitle = "title";
            _syncItem.Published = _published;
            _syncItem.StateKey = _downloadFolder;
            _syncItem.RetryWaitTimeInSeconds = 0;
        }

        protected virtual void SetupStubs()
        {
            _webClient.Setup(client => client.OpenRead(_syncItem.EpisodeUrl)).Returns(_stream);
            _webClientFactory.Setup(factory => factory.CreateWebClient()).Returns(_webClient.Object);
            _directoryInfoProvider.Setup(dir => dir.GetDirectoryInfo(_downloadFolder)).Returns(_directoryInfo.Object);
            _stateProvider.Setup(provider => provider.GetState(_syncItem.StateKey)).Returns(_state.Object);

            _counterFactory.Setup(
                factory => factory.CreateAverageCounter(Constants.PodcastUtilitiesCommonCounterCategory,
                                             Constants.AverageTimeToDownload,
                                             Constants.NumberOfDownloads))
                            .Returns(_averageCounterSize.Object);
            _counterFactory.Setup(
                factory => factory.CreateAverageCounter(Constants.PodcastUtilitiesCommonCounterCategory,
                                             Constants.AverageMBDownload,
                                             Constants.SizeOfDownloads))
                            .Returns(_averageCounterTime.Object);
        }
    }
}
