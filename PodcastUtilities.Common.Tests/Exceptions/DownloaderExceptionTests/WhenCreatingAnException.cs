using NUnit.Framework;
using PodcastUtilities.Common.Exceptions;

namespace PodcastUtilities.Common.Tests.Exceptions.DownloaderExceptionTests
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
