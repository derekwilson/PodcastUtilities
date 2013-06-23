using System;
using NUnit.Framework;
using Rhino.Mocks;

namespace PodcastUtilities.PortableDevices.Tests.DeviceTests
{
    public class WhenGettingRootStorageObjectFromFullPath : WhenTestingDevice
    {
        private IDeviceObject DeviceObject { get; set; }

        protected override void GivenThat()
        {
            base.GivenThat();

            PortableDeviceHelper.Stub(
                helper => helper.GetChildObjectIds(PortableDeviceContent, PortableDeviceConstants.WPD_DEVICE_OBJECT_ID))
                .Return(new[] {"Dummy1", "xId", "Dummy2"});

            PortableDeviceHelper.Stub(
                helper => helper.GetChildObjectIds(PortableDeviceContent, "xId"))
                .Return(new[] {"Dummy3", "yId", "Dummy4"});

            PortableDeviceHelper.Stub(
                helper => helper.GetChildObjectIds(PortableDeviceContent, "yId"))
                .Return(new[] { "Dummy5", "InternalStorageId", "Dummy6" });

            StubObjectFilenameAndContentType("xId", "x", PortableDeviceConstants.WPD_CONTENT_TYPE_FOLDER);
            StubObjectFilenameAndContentType("yId", "y", PortableDeviceConstants.WPD_CONTENT_TYPE_FOLDER);
            StubObjectFilenameAndContentType("InternalStorageId", "Internal Storage", PortableDeviceConstants.WPD_CONTENT_TYPE_FUNCTIONAL_OBJECT);

            PortableDeviceHelper.Stub(
                propertyHelper => propertyHelper.GetGuidProperty(
                    PortableDeviceContent, 
                    "InternalStorageId",
                    PortableDevicePropertyKeys.WPD_FUNCTIONAL_OBJECT_CATEGORY))
                .Return(PortableDeviceConstants.WPD_FUNCTIONAL_CATEGORY_STORAGE);
        }

        private void StubObjectFilenameAndContentType(string objectId, string fileName, Guid contentType)
        {
            PortableDeviceHelper
                .Stub(propertyHelper => propertyHelper.GetObjectFileName(
                    PortableDeviceContent,
                    objectId))
                .Return(fileName);

            PortableDeviceHelper.Stub(
                propertyHelper => propertyHelper.GetObjectContentType(PortableDeviceContent, objectId))
                .Return(contentType);
        }

        protected override void When()
        {
            // The storage object will normally be at the root, but we need to allow for it to be lower in the tree
            DeviceObject = Device.GetRootStorageObjectFromPath(@"x\y\Internal Storage\Foo\Bar");
        }

        [Test]
        public void ItShouldReturnTheCorrectObject()
        {
            Assert.That(DeviceObject.Id, Is.EqualTo("InternalStorageId"));
            Assert.That(DeviceObject.Name, Is.EqualTo("Internal Storage"));
        }
    }
}