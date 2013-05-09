using NUnit.Framework;
using PodcastUtilities.Common.Configuration;
using Rhino.Mocks;

namespace PodcastUtilities.Common.Tests.Configuration.PodcastFactoryTests
{
    public class WhenCreatingAPodcast
        : WhenTestingBehaviour
    {
        protected IReadOnlyControlFile _controlFile;

        protected PodcastFactory PodcastFactory { get; set; }

        protected IPodcastInfo CreatedPodcast { get; set; }

        protected override void GivenThat()
        {
            base.GivenThat();

            _controlFile = TestControlFileFactory.CreateControlFile();

            PodcastFactory = new PodcastFactory();
        }

        protected override void When()
        {
            CreatedPodcast = PodcastFactory.CreatePodcast(_controlFile);
        }

        [Test]
        public void ItShouldCreateAPodcastObject()
        {
            Assert.That(CreatedPodcast, Is.Not.Null);
        }

        [Test]
        public void ItShouldCreateAPodcastFeedObject()
        {
            Assert.That(CreatedPodcast.Feed, Is.Not.Null);
        }
    }
}