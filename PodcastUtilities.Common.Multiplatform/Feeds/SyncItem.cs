﻿#region License
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
using PodcastUtilities.Common.Configuration;

namespace PodcastUtilities.Common.Feeds
{
    /// <summary>
    /// an item to be downloaded
    /// </summary>
    public class SyncItem : ISyncItem
    {
        /// <summary>
        /// unique id of the download
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// date time the episode was published
        /// </summary>
        public DateTime Published { get; set; }

        /// <summary>
        /// state key - used to find the state of the podcast feed
        /// at the moment its the folder conatining the feed items and in that folder we can find the state XML file
        /// </summary>
        public string StateKey { get; set; }

        /// <summary>
        /// the url to download from
        /// </summary>
        public Uri EpisodeUrl { get; set; }

        /// <summary>
        /// pathname to be downloaded to
        /// </summary>
        public string DestinationPath { get; set; }

        /// <summary>
        /// the title of the eposide
        /// </summary>
        public string EpisodeTitle { get; set; }

        /// <summary>
        /// time to wait if there is a file lock on state
        /// </summary>
        public int RetryWaitTimeInSeconds { get; set; }

        /// <summary>
        /// command to execute after the download
        /// </summary>
        public IExternalCommand PostDownloadCommand { get; set; }
    }
}