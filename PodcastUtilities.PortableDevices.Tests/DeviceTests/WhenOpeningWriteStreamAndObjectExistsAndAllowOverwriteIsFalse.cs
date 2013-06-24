using System.IO;
using NUnit.Framework;

namespace PodcastUtilities.PortableDevices.Tests.DeviceTests
{
    public class WhenOpeningWriteStreamAndObjectExistsAndAllowOverwriteIsFalse : WhenOpeningStream
    {
        protected override void DoWhen()
        {
            OpenedStream = Device.OpenWrite(@"Internal Storage\foo.mp3", 1234, false);
        }

        [Test]
        public void ItShouldThrowIOException()
        {
            Assert.That(ThrownException, Is.InstanceOf<IOException>());
        }
    }
}