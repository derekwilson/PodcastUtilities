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
    /// an individual podcast
    /// </summary>
    public interface IPodcastInfo : ICloneable, IXmlSerializable
    {
        /// <summary>
        /// the folder relative to the source root that contains the media for the podcast
        /// </summary>
        string Folder { get; set; }

        /// <summary>
        /// file pattern for the media files eg. *.mp3
        /// </summary>
        IDefaultableItem<string> Pattern { get; set; }

        /// <summary>
        /// true if we should delete a folder when all the podcasts have been removed
        /// </summary>
        IDefaultableItem<bool> DeleteEmptyFolder { get; set; }
        
        /// <summary>
        /// field to sort on "creationtime" to use the file created time anything else to use the file name
        /// </summary>
        IDefaultableItem<PodcastFileSortField> SortField { get; set; }

        /// <summary>
        /// true for an ascending sort, false for a descending
        /// </summary>
        IDefaultableItem<bool> AscendingSort { get; set; }

        /// <summary>
        /// maximum number of files to copy, -1 for unlimited
        /// </summary>
        IDefaultableItem<int> MaximumNumberOfFiles { get; set; }

        /// <summary>
        /// the configuration info for the feed
        /// </summary>
        IFeedInfo Feed { get; set; }

        /// <summary>
        /// create a feed in the podcast
        /// </summary>
        void CreateFeed();

        /// <summary>
        /// remove a feed from the podcast
        /// </summary>
        void RemoveFeed();

        /// <summary>
        /// create a post download command in the podcast
        /// </summary>
        void CreatePostDownloadCommand();

        /// <summary>
        /// remove a post download command from the podcast
        /// </summary>
        void RemovePostDownloadCommand();
    }
}