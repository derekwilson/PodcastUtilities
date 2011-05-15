using System.Collections.Generic;
using NUnit.Framework;

namespace PodcastUtilities.Common.Tests.PodcastFeedInRssFormatTests
{
    public class WhenLoadingTheEpisodesFromExampleXml : WhenTestingTheFeed
    {
        private List<IPodcastFeedItem> _episodes;

        protected override void GivenThat()
        {
            base.GivenThat();
            Feed = new PodcastFeedInRssFormat(FeedXmlStream);
        }

        protected override void When()
        {
            _episodes = Feed.GetFeedEpisodes();
        }

        [Test]
        public void ItShouldLoadTheEpisodes()
        {
            Assert.That(_episodes.Count, Is.EqualTo(14));
        }

        [Test]
        public void ItShouldExcludeIllegalFilenames()
        {
            Assert.That(_episodes[0].GetFilename(), Is.EqualTo("15-Revolt_.mp3"));
            Assert.That(_episodes[2].GetFilename(), Is.EqualTo("___"));
        }

        [Test]
        public void ItShouldCopeWithSimpleUrls()
        {
            Assert.That(_episodes[1].GetFilename(), Is.EqualTo("114_Obsession.mp3"));
        }

        [Test]
        public void ItShouldEliminateEmptyUrl()
        {
            Assert.That(_episodes[3].GetFilename(), Is.EqualTo("11-Scars.mp3"));
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