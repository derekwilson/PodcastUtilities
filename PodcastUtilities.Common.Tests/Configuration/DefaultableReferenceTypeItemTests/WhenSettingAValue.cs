using NUnit.Framework;
using PodcastUtilities.Common.Configuration;

namespace PodcastUtilities.Common.Tests.Configuration.DefaultableReferenceTypeItemTests
{
    public class WhenSettingAValue : WhenTestingAnItem
    {
        protected override void GivenThat()
        {
            base.GivenThat();
            _item = new DefaultableReferenceTypeItem<string>(DefaultProvider);
        }

        protected override void When()
        {
            _item.Value = "new value";
        }

        [Test]
        public void ItShouldSetTheValue()
        {
            Assert.That(_item.IsSet, Is.EqualTo(true));
        }

        [Test]
        public void ItShouldReturnTheValue()
        {
            Assert.That(_item.Value, Is.EqualTo("new value"));
        }
    }
}
