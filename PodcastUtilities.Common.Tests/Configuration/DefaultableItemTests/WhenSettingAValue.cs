using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using PodcastUtilities.Common.Configuration;

namespace PodcastUtilities.Common.Tests.Configuration.DefaultableItemTests
{
    public class WhenSettingAValue : WhenTestingAnItem
    {
        protected override void GivenThat()
        {
            base.GivenThat();
            _item = new DefaultableItem<int>(DefaultProvider);
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
