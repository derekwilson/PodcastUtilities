using PodcastUtilities.Common.Feeds;

namespace PodcastUtilities.Common.Tests.Feeds.CommandGeneratorTests
{
    public abstract class WhenTestingTheGenerator
        : WhenTestingBehaviour
    {
        protected CommandGenerator Generator { get; set; }

        protected IExternalCommand GeneratedCommand { get; set; }

        protected override void GivenThat()
        {
            base.GivenThat();

            Generator = new CommandGenerator();
        }
    }
}