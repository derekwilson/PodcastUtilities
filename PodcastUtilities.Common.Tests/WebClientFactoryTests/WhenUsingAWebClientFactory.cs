using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using PodcastUtilities.Common.IO;

namespace PodcastUtilities.Common.Tests.WebClientFactoryTests
{
    public class WhenUsingAWebClientFactory
                : WhenTestingBehaviour
    {
        private IWebClientFactory _factory;
        private IWebClient _client;

        protected override void GivenThat()
        {
            base.GivenThat();
            _factory = new WebClientFactory();
        }

        protected override void When()
        {
            _client = _factory.GetWebClient();
        }

        [Test]
        public void ItShouldReturnAWebClient()
        {
            Assert.IsInstanceOf(typeof(IWebClient),_client);
        }
    }
}
