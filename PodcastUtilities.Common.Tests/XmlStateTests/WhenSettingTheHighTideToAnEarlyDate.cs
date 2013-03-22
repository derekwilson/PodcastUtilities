using System;
using NUnit.Framework;

namespace PodcastUtilities.Common.Tests.XmlStateTests
{
    public class WhenSettingTheHighTideToAnEarlyDate : WhenUsingTestState
    {
        protected override void GivenThat()
        {
            base.GivenThat();
            _state = new XmlState(_testXmlDocument);
        }

        protected override void When()
        {
            _state.DownloadHighTide = new DateTime(2011, 7, 10, 9, 11, 12);
        }

        [Test]
        public void ItShouldNotUpdateTheState()
        {
            Assert.That(_state.DownloadHighTide, Is.EqualTo(new DateTime(2011, 7, 10, 10, 11, 12)));
        }
    }
}