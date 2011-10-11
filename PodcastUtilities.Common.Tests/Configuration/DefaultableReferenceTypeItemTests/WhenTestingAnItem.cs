using PodcastUtilities.Common.Configuration;

namespace PodcastUtilities.Common.Tests.Configuration.DefaultableReferenceTypeItemTests
{
    public abstract class WhenTestingAnItem
        : WhenTestingBehaviour
    {
        protected IDefaultableItem<string> _item;
        protected string _defaultValue;

        protected string DefaultProvider()
        {
            return _defaultValue;
        }

        protected override void GivenThat()
        {
            base.GivenThat();

            _defaultValue = "default";
        }
    }
}