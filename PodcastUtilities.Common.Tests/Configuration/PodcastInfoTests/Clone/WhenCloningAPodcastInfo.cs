using PodcastUtilities.Common.Configuration;

namespace PodcastUtilities.Common.Tests.Configuration.PodcastInfoTests.Clone
{
    public abstract class WhenCloningAPodcastInfo
        : WhenTestingBehaviour
    {
        protected IReadOnlyControlFile _controlFile;

        protected PodcastInfo _pocastInfo;
        protected PodcastInfo _clonedPodcast;

        protected override void GivenThat()
        {
            base.GivenThat();

            _controlFile = TestControlFileFactory.CreateControlFile();
            _pocastInfo = new PodcastInfo(_controlFile);
        }
    }
}