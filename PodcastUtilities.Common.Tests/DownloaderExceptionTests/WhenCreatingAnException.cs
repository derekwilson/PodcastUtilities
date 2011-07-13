using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace PodcastUtilities.Common.Tests.DownloaderExceptionTests
{
    public class WhenCreatingAnException : WhenTestingBehaviour
    {
        protected DownloaderException Exception { get; set; }

        protected override void When()
        {
            Exception = new DownloaderException("TESTMESSAGE");
        }

        [Test]
        public void ItShouldSetTheMessage()
        {
            Assert.That(Exception.Message,Is.EqualTo("TESTMESSAGE"));
        }
    }
}
