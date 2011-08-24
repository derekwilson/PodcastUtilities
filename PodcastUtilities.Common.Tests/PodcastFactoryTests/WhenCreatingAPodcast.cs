using NUnit.Framework;
using Rhino.Mocks;

namespace PodcastUtilities.Common.Tests.PodcastFactoryTests
{
    public class WhenCreatingAPodcast
        : WhenTestingBehaviour
    {
        protected IPodcastDefaultsProvider DefaultsProvider { get; set; }

        protected PodcastFactory PodcastFactory { get; set; }

        protected PodcastInfo CreatedPodcast { get; set; }

        protected override void GivenThat()
        {
            base.GivenThat();

            DefaultsProvider = GenerateMock<IPodcastDefaultsProvider>();

            DefaultsProvider.Stub(p => p.Pattern).Return("*.blah");
            DefaultsProvider.Stub(p => p.SortField).Return("name");
            DefaultsProvider.Stub(p => p.AscendingSort).Return(true);
            DefaultsProvider.Stub(p => p.FeedFormat).Return(PodcastFeedFormat.ATOM);
            DefaultsProvider.Stub(p => p.EpisodeNamingStyle).Return(PodcastEpisodeNamingStyle.UrlFilenameAndPublishDateTime);
            DefaultsProvider.Stub(p => p.EpisodeDownloadStrategy).Return(PodcastEpisodeDownloadStrategy.HighTide);
            DefaultsProvider.Stub(p => p.MaximumDaysOld).Return(33);

            PodcastFactory = new PodcastFactory(DefaultsProvider);
        }

        protected override void When()
        {
            CreatedPodcast = PodcastFactory.CreatePodcast();
        }

        [Test]
        public void ItShouldCreateAPodcastObject()
        {
            Assert.That(CreatedPodcast, Is.Not.Null);
        }

        [Test]
        public void ItShouldInitializeThePodcastUsingDefaults()
        {
            Assert.That(CreatedPodcast.Pattern, Is.EqualTo("*.blah"));
            Assert.That(CreatedPodcast.SortField, Is.EqualTo("name"));
            Assert.That(CreatedPodcast.AscendingSort, Is.True);
            Assert.That(CreatedPodcast.Feed.Format, Is.EqualTo(PodcastFeedFormat.ATOM));
            Assert.That(CreatedPodcast.Feed.NamingStyle, Is.EqualTo(PodcastEpisodeNamingStyle.UrlFilenameAndPublishDateTime));
            Assert.That(CreatedPodcast.Feed.DownloadStrategy, Is.EqualTo(PodcastEpisodeDownloadStrategy.HighTide));
            Assert.That(CreatedPodcast.Feed.MaximumDaysOld, Is.EqualTo(33));
        }
    }
}