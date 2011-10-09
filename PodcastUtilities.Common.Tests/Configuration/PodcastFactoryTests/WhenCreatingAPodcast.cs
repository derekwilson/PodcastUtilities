using NUnit.Framework;
using PodcastUtilities.Common.Configuration;
using Rhino.Mocks;

namespace PodcastUtilities.Common.Tests.Configuration.PodcastFactoryTests
{
    public class WhenCreatingAPodcast
        : WhenTestingBehaviour
    {
        protected IReadOnlyControlFile _controlFile;
        protected IPodcastDefaultsProvider DefaultsProvider { get; set; }

        protected PodcastFactory PodcastFactory { get; set; }

        protected IPodcastInfo CreatedPodcast { get; set; }

        protected override void GivenThat()
        {
            base.GivenThat();

            _controlFile = TestControlFileFactory.CreateControlFile();

            DefaultsProvider = GenerateMock<IPodcastDefaultsProvider>();

            DefaultsProvider.Stub(p => p.Pattern).Return("*.blah");
            PodcastFactory = new PodcastFactory(DefaultsProvider);
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
        public void ItShouldInitializeThePodcastUsingDefaultsPattern()
        {
            Assert.That(CreatedPodcast.Pattern, Is.EqualTo("*.blah"));
        }
    }
}