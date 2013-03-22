using PodcastUtilities.Common.Configuration;

namespace PodcastUtilities.Common.Tests.Configuration.DefaultableValueTypeItemTests
{
    public abstract class WhenTestingEquality : WhenTestingAnItem
    {
        protected IDefaultableItem<int> _item2;

        protected bool _itemsAreEqual;

        protected override void When()
        {
            _itemsAreEqual = _item.Equals(_item2);
        }
    }
}