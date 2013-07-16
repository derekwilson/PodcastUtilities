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
using NUnit.Framework;
using PodcastUtilities.Common.Feeds;
using PodcastUtilities.Common.Platform;

namespace PodcastUtilities.Common.Tests.Feeds.EpisodeDownloaderFactoryTests
{
    public class WhenUsingAnEpisodeDownloaderFactory
                : WhenTestingBehaviour
    {
        private IEpisodeDownloaderFactory _factory;
        private IDirectoryInfoProvider _directoryInfoProvider;
        private IFileUtilities _fileUtilities;
        private IWebClientFactory _webClientFactory;
        private IEpisodeDownloader _downloader;
        private IStateProvider _stateProvider;
        protected ICounterFactory _counterFactory;
        private ICommandExecuter _commandExecuter;

        protected override void GivenThat()
        {
            base.GivenThat();
            _counterFactory = GenerateMock<ICounterFactory>();
            _stateProvider = GenerateMock<IStateProvider>();
            _webClientFactory = new WebClientFactory();
            _directoryInfoProvider = GenerateMock<IDirectoryInfoProvider>();
            _fileUtilities = GenerateMock<IFileUtilities>();
            _commandExecuter = GenerateMock<ICommandExecuter>();
            _factory = new EpisodeDownloaderFactory(_webClientFactory, _directoryInfoProvider, _fileUtilities, _stateProvider, _counterFactory, _commandExecuter);
        }

        protected override void When()
        {
            _downloader = _factory.CreateDownloader();
        }

        [Test]
        public void ItShouldReturnADownloader()
        {
            Assert.IsInstanceOf(typeof(IEpisodeDownloader), _downloader);
        }
    }
}
