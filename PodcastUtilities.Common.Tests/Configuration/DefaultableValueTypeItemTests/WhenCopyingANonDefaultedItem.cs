using NUnit.Framework;
using PodcastUtilities.Common.Configuration;

namespace PodcastUtilities.Common.Tests.Configuration.DefaultableValueTypeItemTests
{
    public class WhenCopyingANonDefaultedItem : WhenTestingAnItem
    {
        private DefaultableValueTypeItem<int> _item2;

        protected override void GivenThat()
        {
            base.GivenThat();

            _item = new DefaultableValueTypeItem<int>(DefaultProvider) {Value = 777};

            _item2 = new DefaultableValueTypeItem<int>(DefaultProvider) {Value = 333};
        }

        protected override void When()
        {
            _item.Copy(_item2);
        }

        [Test]
        public void ItShouldCopyTheValue()
        {
            Assert.That(_item.Value, Is.EqualTo(333));
        }
    }
}