using PodcastUtilities.Common.Configuration;

namespace PodcastUtilities.Common.Tests.Configuration.FeedInfoTests.Clone
{
    public abstract class WhenCloningAFeedInfo
        : WhenTestingBehaviour
    {
        protected IReadOnlyControlFile _controlFile;

        protected FeedInfo _feedInfo;
        protected FeedInfo _clonedFeedInfo;

        protected override void GivenThat()
        {
            base.GivenThat();

            _controlFile = TestControlFileFactory.CreateControlFile();
            _feedInfo = new FeedInfo(_controlFile);
        }
    }
}