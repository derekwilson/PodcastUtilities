using NUnit.Framework;
using Rhino.Mocks;

namespace PodcastUtilities.PortableDevices.Tests.DeviceTests
{
    public class WhenGettingObjectFromFullPath : WhenTestingDevice
    {
        private IDeviceObject DeviceObject { get; set; }

        protected override void GivenThat()
        {
            base.GivenThat();

            PortableDeviceHelper.Stub(
                helper => helper.GetChildObjectIds(PortableDeviceContent, PortableDeviceConstants.WPD_DEVICE_OBJECT_ID))
                .Return(new[] {"Dummy1", "InternalStorageID", "Dummy2"});

            PortableDeviceHelper.Stub(
                helper => helper.GetChildObjectIds(PortableDeviceContent, "InternalStorageID"))
                .Return(new[] {"Dummy3", "Dummy4", "fooId"});

            PortableDeviceHelper.Stub(
                helper => helper.GetChildObjectIds(PortableDeviceContent, "fooId"))
                .Return(new[] {"Dummy5", "Dummy6", "barId"});

            PortableDeviceHelper
                .Stub(propertyHelper => propertyHelper.GetObjectName(
                    PortableDeviceContent,
                    "InternalStorageID"))
                .Return("Internal Storage");

            PortableDeviceHelper
                .Stub(propertyHelper => propertyHelper.GetObjectName(
                    PortableDeviceContent,
                    "fooId"))
                .Return("foo");

            PortableDeviceHelper
                .Stub(propertyHelper => propertyHelper.GetObjectName(
                    PortableDeviceContent,
                    "barId"))
                .Return("bar");
        }

        protected override void When()
        {
            DeviceObject = Device.GetObjectFromPath(@"Internal Storage\Foo\Bar");
        }

        [Test]
        public void ItShouldReturnTheCorrectObject()
        {
            Assert.That(DeviceObject.Id, Is.EqualTo("barId"));
            Assert.That(DeviceObject.Name, Is.EqualTo("bar"));
        }
    }
}