using NUnit.Framework;
using PodcastUtilities.Common.Configuration;

namespace PodcastUtilities.Common.Tests.Configuration.DefaultableValueTypeItemTests
{
    public class WhenRevertingValue : WhenTestingAnItem
    {
        protected override void GivenThat()
        {
            base.GivenThat();
            _item = new DefaultableValueTypeItem<int>(DefaultProvider);
            _item.Value = 456;
        }

        protected override void When()
        {
            _item.RevertToDefault();
        }

        [Test]
        public void ItShouldNotSetTheValue()
        {
            Assert.That(_item.IsSet, Is.EqualTo(false));
        }

        [Test]
        public void ItShouldReturnTheDefaultValue()
        {
            Assert.That(_item.Value, Is.EqualTo(123));
        }
    }

}
