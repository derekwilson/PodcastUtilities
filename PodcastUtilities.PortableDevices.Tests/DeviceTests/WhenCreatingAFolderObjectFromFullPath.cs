using NUnit.Framework;
using PortableDeviceApiLib;
using Rhino.Mocks;

namespace PodcastUtilities.PortableDevices.Tests.DeviceTests
{
    public class WhenCreatingAFolderObjectFromFullPath : WhenTestingDevice
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
                .Stub(propertyHelper => propertyHelper.GetObjectFileName(
                    PortableDeviceContent,
                    "InternalStorageID"))
                .Return("Internal Storage");

            PortableDeviceHelper
                .Stub(propertyHelper => propertyHelper.GetObjectFileName(
                    PortableDeviceContent,
                    "fooId"))
                .Return("foo");

            PortableDeviceHelper
                .Stub(propertyHelper => propertyHelper.GetObjectFileName(
                    PortableDeviceContent,
                    "barId"))
                .Return("bar");
        }

        protected override void When()
        {
            Device.CreateFolderObjectFromPath(@"Internal Storage\Foo\NewFolder");
        }

        [Test]
        public void ItShouldCreateTheCorrectFolder()
        {
            PortableDeviceHelper.AssertWasCalled(
                helper => helper.CreateFolderObject(
                                    Arg<IPortableDeviceContent>.Is.Equal(PortableDeviceContent), 
                                    Arg<string>.Is.Equal("fooId"), 
                                    Arg<string>.Is.Anything));
        }

        [Test]
        public void ItShouldGetTheNewFolder()
        {
            PortableDeviceHelper.AssertWasCalled(
                helper => helper.GetChildObjectIds(
                                    Arg<IPortableDeviceContent>.Is.Equal(PortableDeviceContent),
                                    Arg<string>.Is.Equal("fooId")
                                    ), o => o.Repeat.Times(2));
        }
    }
}