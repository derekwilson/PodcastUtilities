using PodcastUtilities.Common.Configuration;

namespace PodcastUtilities.Common.Tests.Configuration.DefaultableItemTests
{
    public abstract class WhenTestingAnItem
        : WhenTestingBehaviour
    {
        protected IDefaultableItem<int> _item;
        protected int _defaultValue;

        protected int DefaultProvider()
        {
            return _defaultValue;
        }

        protected override void GivenThat()
        {
            base.GivenThat();

            _defaultValue = 123;
        }
    }
}