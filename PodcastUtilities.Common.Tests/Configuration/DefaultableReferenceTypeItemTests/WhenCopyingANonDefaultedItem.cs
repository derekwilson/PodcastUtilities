using NUnit.Framework;
using PodcastUtilities.Common.Configuration;

namespace PodcastUtilities.Common.Tests.Configuration.DefaultableReferenceTypeItemTests
{
    public class WhenCopyingANonDefaultedItem : WhenTestingAnItem
    {
        private DefaultableReferenceTypeItem<string> _item2;

        protected override void GivenThat()
        {
            base.GivenThat();

            _item = new DefaultableReferenceTypeItem<string>(DefaultProvider) {Value = "111"};
            _item2 = new DefaultableReferenceTypeItem<string>(DefaultProvider) {Value = "222"};
        }

        protected override void When()
        {
            _item.Copy(_item2);
        }

        [Test]
        public void ItShouldCopyTheValue()
        {
            Assert.That(_item.Value, Is.EqualTo("222"));
        }
    }
}