﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using PodcastUtilities.Common.Platform;
using Rhino.Mocks;

namespace PodcastUtilities.Common.Tests.PodcastEpisodeDownloaderTests
{
    public abstract class WhenTestingTheDownloader
        : WhenTestingBehaviour
    {
        protected PodcastEpisodeDownloader _downloader;

        protected IWebClientFactory _webClientFactory;
        protected IWebClient _webClient;

        protected IFeedSyncItem _syncItem;

        protected MemoryStream _stream;

        protected System.Exception _exception;

        protected StatusUpdateEventArgs _statusUpdateArgs;
        protected ProgressEventArgs _progressUpdateArgs;

        protected IStateProvider _stateProvider;

        protected IDirectoryInfoProvider _directoryInfoProvider;
        protected IFileUtilities _fileUtilities;
        protected IDirectoryInfo _directoryInfo;
        protected string _downloadFolder;
        protected IState _state;
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
            _state = GenerateMock<IState>();

            _syncItem = new FeedSyncItem();
            _exception = null;

            SetupData();
            SetupStubs();

            _downloader = new PodcastEpisodeDownloader(_webClientFactory, _directoryInfoProvider,_fileUtilities,_stateProvider);
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
            _published = new DateTime(2011,10,15,16,15,14);

            _syncItem.EpisodeUrl = new Uri("http://test");
            _syncItem.DestinationPath = Path.Combine(_downloadFolder,"file.ext");
            _syncItem.EpisodeTitle = "title";
            _syncItem.Published = _published;
            _syncItem.StateKey = _downloadFolder;
        }

        protected virtual void SetupStubs()
        {
            _webClient.Stub(client => client.OpenRead(_syncItem.EpisodeUrl)).Return(_stream);
            _webClientFactory.Stub(factory => factory.GetWebClient()).Return(_webClient);
            _directoryInfoProvider.Stub(dir => dir.GetDirectoryInfo(_downloadFolder)).Return(_directoryInfo);
            _stateProvider.Stub(provider => provider.GetState(_syncItem.StateKey)).Return(_state);
        }
    }
}