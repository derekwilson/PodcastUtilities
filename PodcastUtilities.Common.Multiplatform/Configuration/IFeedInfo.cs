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
using System.Xml.Serialization;

namespace PodcastUtilities.Common.Configuration
{
    /// <summary>
    /// configuration info for a podcast feed
    /// </summary>
    public interface IFeedInfo : ICloneable, IXmlSerializable
    {
        /// <summary>
        /// the address of the podcast feed
        /// </summary>
        Uri Address { get; set; }

        /// <summary>
        /// the format the feed is in
        /// </summary>
        IDefaultableItem<PodcastFeedFormat> Format { get; set; }

        /// <summary>
        /// do not download podcasts that werre published before this number of days ago
        /// </summary>
        IDefaultableItem<int> MaximumDaysOld { get; set; }

        /// <summary>
        /// the naming style to use for episodes downloaded from the feed
        /// </summary>
        IDefaultableItem<PodcastEpisodeNamingStyle> NamingStyle { get; set; }

        /// <summary>
        /// the strategy to be used when downloading episodes
        /// </summary>
        IDefaultableItem<PodcastEpisodeDownloadStrategy> DownloadStrategy { get; set; }

        /// <summary>
        /// number of days before we delete a download
        /// </summary>
        IDefaultableItem<int> DeleteDownloadsDaysOld { get; set; }

        /// <summary>
        /// maximum number of items from the feed to have in the cache
        /// </summary>
        IDefaultableItem<int> MaximumNumberOfDownloadedItems { get; set; }
    }
}