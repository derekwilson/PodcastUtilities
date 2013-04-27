using PodcastUtilities.Common.Tests;

namespace PodcastUtilities.PortableDevices.Tests.FilenameMatcherTests
{
    public abstract class WhenTestingFilenameMatcher
        : WhenTestingBehaviour
    {
        protected FilenameMatcher FilenameMatcher { get; set; }

        protected override void GivenThat()
        {
            base.GivenThat();

            FilenameMatcher = new FilenameMatcher();
        }

        protected override void When()
        {
        }
    }
}