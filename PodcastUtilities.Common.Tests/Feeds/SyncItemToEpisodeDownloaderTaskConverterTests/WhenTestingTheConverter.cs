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
using System.Collections.Generic;
using PodcastUtilities.Common.Feeds;
using PodcastUtilities.Common.Platform;

namespace PodcastUtilities.Common.Tests.Feeds.SyncItemToEpisodeDownloaderTaskConverterTests
{
    public abstract class WhenTestingTheConverter
        : WhenTestingBehaviour
    {
        protected SyncItemToEpisodeDownloaderTaskConverter _converter;

        protected IWebClientFactory _webClientFactory;
        protected IDirectoryInfoProvider _directoryInfoProvider;
        protected IFileUtilities _fileUtilities;
        protected IStateProvider _stateProvider;
        protected ICounterFactory _counterFactory;
        protected ICommandExecuter _commandExecuter;

        protected IEpisodeDownloaderFactory _downloaderFactory;

        protected List<ISyncItem> _downloadItems;
        protected IEpisodeDownloader[] _tasks;

        protected override void GivenThat()
        {
            base.GivenThat();

            _counterFactory = GenerateMock<ICounterFactory>();
            _stateProvider = GenerateMock<IStateProvider>();
            _webClientFactory = GenerateMock<IWebClientFactory>();
            _commandExecuter = GenerateMock<ICommandExecuter>();
            _downloaderFactory = new EpisodeDownloaderFactory(_webClientFactory, _directoryInfoProvider, _fileUtilities, _stateProvider, _counterFactory, _commandExecuter);
            _downloadItems = new List<ISyncItem>(10);

            SetupData();
            SetupStubs();

            _converter = new SyncItemToEpisodeDownloaderTaskConverter(_downloaderFactory);
        }

        protected virtual void SetupData()
        {
        }

        protected virtual void SetupStubs()
        {

        }
    }
}
