using NUnit.Framework;
using PodcastUtilities.Common.Configuration;

namespace PodcastUtilities.Common.Tests.Configuration.DefaultableValueTypeItemTests
{
    public class WhenComparingWithObjectOfDifferentType : WhenTestingEquality
    {
        protected override void GivenThat()
        {
            base.GivenThat();

            _item = new DefaultableValueTypeItem<int>(DefaultProvider) { Value = 999 };
        }

        protected override void When()
        {
            _itemsAreEqual = _item.Equals("blah");
        }

        [Test]
        public void ItShouldReturnNotEqual()
        {
            Assert.That(!_itemsAreEqual);
        }
    }
}