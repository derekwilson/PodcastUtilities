using System;
using NUnit.Framework;

namespace PodcastUtilities.Common.Tests.XmlStateTests
{
    public class WhenSettingTheHighTide : WhenUsingTestState
    {
        protected override void GivenThat()
        {
            base.GivenThat();
            _state = new XmlState(_testXmlDocument);
        }

        protected override void When()
        {
            _state.DownloadHighTide = new DateTime(2011, 7, 10, 16, 11, 12);
        }

        [Test]
        public void ItShouldUpdateTheState()
        {
            Assert.That(_state.DownloadHighTide, Is.EqualTo(new DateTime(2011, 7, 10, 16, 11, 12)));
        }
    }
}