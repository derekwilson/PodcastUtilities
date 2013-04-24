using NUnit.Framework;
using Rhino.Mocks;

namespace PodcastUtilities.PortableDevices.Tests.DeviceTests
{
    public class WhenGettingObjectFromRootObjectPath : WhenTestingDevice
    {
        private IDeviceObject DeviceObject { get; set; }

        protected override void GivenThat()
        {
            base.GivenThat();

            PortableDeviceHelper.Stub(
                helper => helper.GetChildObjectIds(PortableDeviceContent, PortableDeviceConstants.WPD_DEVICE_OBJECT_ID))
                .Return(new[] {"Dummy1", "InternalStorageID", "Dummy2"});

            PortableDeviceHelper
                .Stub(propertyHelper => propertyHelper.GetObjectFileName(
                    PortableDeviceContent,
                    "InternalStorageID"))
                .Return("Internal Storage");
        }

        protected override void When()
        {
            DeviceObject = Device.GetObjectFromPath("Internal Storage");
        }

        [Test]
        public void ItShouldReturnTheCorrectObject()
        {
            Assert.That(DeviceObject.Id, Is.EqualTo("InternalStorageID"));
            Assert.That(DeviceObject.Name, Is.EqualTo("Internal Storage"));
        }
    }
}