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
using System.IO;
using PodcastUtilities.Common.Configuration;
using PodcastUtilities.Common.Exceptions;

namespace PodcastUtilities.Common.Feeds
{
    /// <summary>
    /// factory to get a feed
    /// </summary>
    public class PodcastFeedFactory : IPodcastFeedFactory
    {
        /// <summary>
        /// construct a feed from the supplied data in the specified format
        /// </summary>
        /// <param name="playlistFormat">the format of the data</param>
        /// <param name="feedData">the data for the feed</param>
        /// <param name="retainCopyFileName">if present then save a copy of the feed xml before parsing - null to just load</param>
        /// <returns>a podcast feed object</returns>
        public IPodcastFeed CreatePodcastFeed(PodcastFeedFormat playlistFormat, Stream feedData, string retainCopyFileName)
        {
            switch (playlistFormat)
            {
                case PodcastFeedFormat.RSS:
                    return new PodcastFeedInRssFormat(feedData, retainCopyFileName);
                default:
                    throw new EnumOutOfRangeException("playlistFormat");
            }
        }
    }
}