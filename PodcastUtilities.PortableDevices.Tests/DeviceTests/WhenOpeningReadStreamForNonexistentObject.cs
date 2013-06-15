using System.IO;
using NUnit.Framework;

namespace PodcastUtilities.PortableDevices.Tests.DeviceTests
{
    public class WhenOpeningReadStreamForNonexistentObject : WhenOpeningStream
    {
        protected override void DoWhen()
        {
            OpenedStream = Device.OpenRead(@"Internal Storage\nonexistent");
        }

        [Test]
        public void ItShouldThrowFileNotFoundException()
        {
            Assert.That(ThrownException, Is.InstanceOf<FileNotFoundException>());
        }
    }
}