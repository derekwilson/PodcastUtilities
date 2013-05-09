using NUnit.Framework;

namespace PodcastUtilities.PortableDevices.Tests.DeviceManagerTests
{
    public class WhenGettingDeviceThatDoesNotExist : WhenTestingDeviceManager
    {
        protected IDevice Device { get; set; }

        protected override void When()
        {
            Device = DeviceManager.GetDevice("Nonexistent");
        }

        [Test]
        public void ItShouldReturnNull()
        {
            Assert.That(Device, Is.Null);
        }
    }
}