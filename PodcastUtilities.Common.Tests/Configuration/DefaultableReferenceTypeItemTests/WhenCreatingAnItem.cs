using NUnit.Framework;
using PodcastUtilities.Common.Configuration;

namespace PodcastUtilities.Common.Tests.Configuration.DefaultableReferenceTypeItemTests
{
    public class WhenCreatingAnItem : WhenTestingAnItem
    {
        protected override void When()
        {
            _item = new DefaultableReferenceTypeItem<string>(DefaultProvider);
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