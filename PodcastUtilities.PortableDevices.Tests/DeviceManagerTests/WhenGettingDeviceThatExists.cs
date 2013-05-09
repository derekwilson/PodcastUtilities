using NUnit.Framework;
using Rhino.Mocks;

namespace PodcastUtilities.PortableDevices.Tests.DeviceManagerTests
{
    public class WhenGettingDeviceThatExists : WhenTestingDeviceManager
    {
        protected IDevice Device { get; set; }

        protected override void When()
        {
            Device = DeviceManager.GetDevice("Device 2");
        }

        [Test]
        public void ItShouldReturnCreatedDevice()
        {
            Assert.That(Device, Is.Not.Null);
            Assert.That(Device.Id, Is.EqualTo("Device_Id_2"));
            Assert.That(Device.Name, Is.EqualTo("Device 2"));
        }
    }
}