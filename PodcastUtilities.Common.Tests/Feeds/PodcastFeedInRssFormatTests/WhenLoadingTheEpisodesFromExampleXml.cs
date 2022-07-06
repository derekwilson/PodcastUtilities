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
using NUnit.Framework;
using PodcastUtilities.Common.Feeds;

namespace PodcastUtilities.Common.Tests.Feeds.PodcastFeedInRssFormatTests
{
    public class WhenLoadingTheEpisodesFromExampleXml : WhenTestingTheFeed
    {
        private IList<IPodcastFeedItem> _episodes;
        private bool _statusUpdate;
        private bool _statusError;
        private bool _statusWarning;
        private int _completeCount;

        protected override void GivenThat()
        {
            base.GivenThat();
            Feed = new PodcastFeedInRssFormat(FeedXmlStream,null);
            Feed.StatusUpdate += new System.EventHandler<StatusUpdateEventArgs>(Feed_StatusUpdate);
            _statusError = false;
            _statusWarning = false;
            _statusUpdate = false;
            _completeCount = 0;
        }

        void Feed_StatusUpdate(object sender, StatusUpdateEventArgs e)
        {
            switch (e.MessageLevel)
            {
                case StatusUpdateLevel.Warning:
                    _statusWarning = true;
                    break;
                case StatusUpdateLevel.Error:
                    _statusError = true;
                    break;
                case StatusUpdateLevel.Verbose:
                    _statusUpdate = true;
                    break;
            }
            if (e.IsTaskCompletedSuccessfully)
            {
                _completeCount++;
            }
        }

        protected override void When()
        {
            _episodes = Feed.Episodes;
        }

        [Test]
        public void ItShouldLoadTheEpisodes()
        {
            Assert.That(_episodes.Count, Is.EqualTo(14));
        }

        [Test]
        public void ItShouldFireTheStatusEvent()
        {
            Assert.That(_statusError, Is.False);
            Assert.That(_statusWarning, Is.True);
            Assert.That(_statusUpdate, Is.True);
        }

        [Test]
        public void ItShouldFireTheStatusEventCompleteFlagOnce()
        {
            Assert.That(_completeCount, Is.EqualTo(0));
        }

        [Test]
        public void ItShouldExcludeIllegalFilenames()
        {
            Assert.That(_episodes[0].FileName, Is.EqualTo("15-_Revolt_.mp3"));
            Assert.That(_episodes[2].FileName, Is.EqualTo("___"));
        }

        [Test]
        public void ItShouldCopeWithSimpleUrls()
        {
            Assert.That(_episodes[1].FileName, Is.EqualTo("114_Obsession.mp3"));
        }

        [Test]
        public void ItShouldEliminateEmptyUrl()
        {
            Assert.That(_episodes[3].FileName, Is.EqualTo("11-Scars.mp3"));
        }

        [Test]
        public void ItShouldGetThePublishDate()
        {
            Assert.That(_episodes[0].Published.Year, Is.EqualTo(2011));
            Assert.That(_episodes[0].Published.Month, Is.EqualTo(3));
            Assert.That(_episodes[0].Published.Day, Is.EqualTo(22));
            Assert.That(_episodes[0].Published.Hour, Is.EqualTo(17));
            Assert.That(_episodes[0].Published.Minute, Is.EqualTo(17));
            Assert.That(_episodes[0].Published.Second, Is.EqualTo(29));
        }
    }
}