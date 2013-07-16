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
namespace PodcastUtilities.Common.Configuration
{
    /// <summary>
    /// how to name the files as they are downloaded
    /// </summary>
    public enum PodcastEpisodeNamingStyle
    {
        /// <summary>
        /// use the filename part of the url as the name
        /// </summary>
        UrlFileName,
        /// <summary>
        /// use the filename from the url and the date time the episode was published
        /// </summary>
        UrlFileNameAndPublishDateTime,
        /// <summary>
        /// use the filename from the url, the title of the feed and the published date time
        /// </summary>
        UrlFileNameFeedTitleAndPublishDateTime,
        /// <summary>
        /// as UrlFilenameFeedTitleAndPublishDateTime but each month is put in a folder
        /// </summary>
        UrlFileNameFeedTitleAndPublishDateTimeInfolder,
        /// <summary>
        /// use the podcast title as the name
        /// </summary>
        EpisodeTitle,
        /// <summary>
        /// use the published date time and the episode title
        /// </summary>
        EpisodeTitleAndPublishDateTime,
    }
}