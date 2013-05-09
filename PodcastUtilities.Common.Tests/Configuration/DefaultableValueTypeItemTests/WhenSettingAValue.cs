using NUnit.Framework;
using PodcastUtilities.Common.Configuration;

namespace PodcastUtilities.Common.Tests.Configuration.DefaultableValueTypeItemTests
{
    public class WhenSettingAValue : WhenTestingAnItem
    {
        protected override void GivenThat()
        {
            base.GivenThat();
            _item = new DefaultableValueTypeItem<int>(DefaultProvider);
        }

        protected override void When()
        {
            _item.Value = 456;
        }

        [Test]
        public void ItShouldSetTheValue()
        {
            Assert.That(_item.IsSet, Is.EqualTo(true));
        }

        [Test]
        public void ItShouldReturnTheValue()
        {
            Assert.That(_item.Value, Is.EqualTo(456));
        }
    }
}
