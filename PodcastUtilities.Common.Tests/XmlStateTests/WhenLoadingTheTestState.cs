using System;
using System.IO;
using System.Reflection;
using System.Xml;
using NUnit.Framework;

namespace PodcastUtilities.Common.Tests.XmlStateTests
{
    public class WhenLoadingTheTestState : WhenUsingTestState
    {
        protected override void When()
        {
            _state = new XmlState(_testXmlDocument);
        }

        [Test]
        public void ItShouldInitialiseTheState()
        {
            Assert.That(_state.DownloadHighTide, Is.EqualTo(new DateTime(2011, 7, 10, 10, 11, 12)));
        }
    }
}