using NUnit.Framework;
using PodcastUtilities.Common.Configuration;

namespace PodcastUtilities.Common.Tests.Configuration.DefaultableValueTypeItemTests
{
    public class WhenComparingUnequalItems : WhenTestingEquality
    {
        protected override void GivenThat()
        {
            base.GivenThat();

            _item = new DefaultableValueTypeItem<int>(DefaultProvider) {Value = 777};

            _item2 = new DefaultableValueTypeItem<int>(DefaultProvider) {Value = 888};
        }

        [Test]
        public void ItShouldReturnNotEqual()
        {
            Assert.That(!_itemsAreEqual);
        }
    }
}