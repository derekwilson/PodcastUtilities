﻿using System.Collections.Generic;
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
            Feed = new PodcastFeedInRssFormat(FeedXmlStream);
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