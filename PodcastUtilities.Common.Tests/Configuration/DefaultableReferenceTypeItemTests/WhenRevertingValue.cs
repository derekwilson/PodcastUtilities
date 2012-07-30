using NUnit.Framework;
using PodcastUtilities.Common.Configuration;

namespace PodcastUtilities.Common.Tests.Configuration.DefaultableReferenceTypeItemTests
{
    public class WhenRevertingValue : WhenTestingAnItem
    {
        protected override void GivenThat()
        {
            base.GivenThat();
            _item = new DefaultableReferenceTypeItem<string>(DefaultProvider);
            _item.Value = "new value";
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
            Assert.That(_item.Value, Is.EqualTo("default"));
        }
    }

}
