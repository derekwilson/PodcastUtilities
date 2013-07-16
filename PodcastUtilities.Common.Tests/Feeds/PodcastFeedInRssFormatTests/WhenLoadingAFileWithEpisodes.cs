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
using System.IO;
using System.Reflection;
using NUnit.Framework;
using PodcastUtilities.Common.Feeds;

namespace PodcastUtilities.Common.Tests.Feeds.PodcastFeedInRssFormatTests
{
    class WhenLoadingAFileWithEpisodes
        : WhenTestingTheFeed
    {
        private IList<IPodcastFeedItem> _episodes;

        protected override void GivenThat()
        {
            base.GivenThat();
            Feed = new PodcastFeedInRssFormat(FeedXmlStream,null);
        }

        protected override void CreateData()
        {
            base.CreateData();
            FeedXmlResourcePath = "PodcastUtilities.Common.Tests.XML.testbigrssfeed.xml";
            FeedXmlStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(FeedXmlResourcePath);
        }

        protected override void When()
        {
            _episodes = Feed.Episodes;
        }

        [Test]
        public void ItShouldLoadTheEpisodes()
        {
            Assert.That(_episodes.Count, Is.EqualTo(275));
        }

        private void TestFilename(IPodcastFeedItem podcastFeedItem)
        {
            var proposedFilename = string.Format("{0}_{1}",
                                                 podcastFeedItem.Published.ToString("yyyy_MM_dd_HHmm"),
                                                 podcastFeedItem.TitleAsFileName);

            //Debug.WriteLine(string.Format("Filename: {0}",proposedFilename));

            var destinationFolder = Path.Combine(".\\media", "podcast folder");
            var destinationPath = Path.Combine(destinationFolder, proposedFilename);
            Assert.That(destinationPath,Is.Not.Null);
        }

        [Test]
        public void ItShouldHaveTheCorrectPathnames()
        {
            foreach (var podcastFeedItem in _episodes)
            {
                TestFilename(podcastFeedItem);
            }
        }
    }
}