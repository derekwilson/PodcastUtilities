using NUnit.Framework;
using Rhino.Mocks;

namespace PodcastUtilities.PortableDevices.Tests.DeviceTests
{
    public class WhenGettingDeviceName : WhenTestingDevice
    {
        public string DeviceName { get; set; }

        protected override void GivenThat()
        {
            base.GivenThat();

            PortableDeviceHelper
                .Stub(propertyHelper => propertyHelper.GetObjectFileName(
                    PortableDeviceContent,
                    PortableDeviceConstants.WPD_DEVICE_OBJECT_ID))
                .Return("The device name");
        }

        protected override void When()
        {
            DeviceName = Device.Name;
        }

        [Test]
        public void ItShouldReturnTheWpdDeviceObjectNameProperty()
        {
            Assert.That(DeviceName, Is.EqualTo("The device name"));
        }
    }
}