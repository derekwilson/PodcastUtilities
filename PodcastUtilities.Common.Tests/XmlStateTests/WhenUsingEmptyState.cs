using System;
using NUnit.Framework;

namespace PodcastUtilities.Common.Tests.XmlStateTests
{
    public class WhenUsingEmptyState
        : WhenTestingBehaviour
    {
        protected XmlState _state;

        protected override void When()
        {
            _state = new XmlState();
        }

        [Test]
        public void ItShouldInitialiseTheState()
        {
            Assert.That(_state.DownloadHighTide, Is.EqualTo(DateTime.MinValue));
        }
    }
}
