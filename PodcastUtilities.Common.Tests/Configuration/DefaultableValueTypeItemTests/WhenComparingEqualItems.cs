using NUnit.Framework;
using PodcastUtilities.Common.Configuration;

namespace PodcastUtilities.Common.Tests.Configuration.DefaultableValueTypeItemTests
{
    public class WhenComparingEqualItems : WhenTestingEquality
    {
        protected override void GivenThat()
        {
            base.GivenThat();

            _item = new DefaultableValueTypeItem<int>(DefaultProvider) {Value = 555};

            _item2 = new DefaultableValueTypeItem<int>(DefaultProvider) {Value = 555};
        }

        [Test]
        public void ItShouldReturnEqual()
        {
            Assert.That(_itemsAreEqual);
        }
    }
}