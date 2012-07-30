using NUnit.Framework;
using PodcastUtilities.Common.Configuration;

namespace PodcastUtilities.Common.Tests.Configuration.DefaultableValueTypeItemTests
{
    public class WhenCreatingAnItem : WhenTestingAnItem
    {
        protected override void When()
        {
            _item = new DefaultableValueTypeItem<int>(DefaultProvider);
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