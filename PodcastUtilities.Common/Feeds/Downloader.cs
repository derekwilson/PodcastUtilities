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
using PodcastUtilities.Common.Configuration;
using PodcastUtilities.Common.Platform;

namespace PodcastUtilities.Common.Feeds
{
    /// <summary>
    /// object to download the feed channel data
    /// </summary>
    public class Downloader : IDownloader
    {
        private IWebClient _webClient;
        private IPodcastFeedFactory _feedFactory;

        /// <summary>
        /// create a downloader
        /// </summary>
        /// <param name="webClient">access to the internet</param>
        /// <param name="feedFactory">factory to create a feed</param>
        public Downloader(IWebClient webClient, IPodcastFeedFactory feedFactory)
        {
            _webClient = webClient;
            _feedFactory = feedFactory;
        }

        /// <summary>
        /// populate a PodcastFeed from the supplied URI
        /// </summary>
        /// <param name="feedFormat">the format of the feed</param>
        /// <param name="address">the url to get the feed from</param>
        /// <param name="retainCopyFileName">if present then save a copy of the feed xml before parsing - null to just load</param>
        /// <returns>the podcast feed</returns>
        public IPodcastFeed DownloadFeed(PodcastFeedFormat feedFormat, Uri address, string retainCopyFileName)
        {
            Stream feedData = _webClient.OpenRead(address);

            return _feedFactory.CreatePodcastFeed(feedFormat, feedData, retainCopyFileName);
        }
    }
}
