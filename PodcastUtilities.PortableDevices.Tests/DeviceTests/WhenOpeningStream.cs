using System;
using System.IO;
using PortableDeviceApiLib;
using Rhino.Mocks;
using R = Rhino.Mocks.Constraints;

namespace PodcastUtilities.PortableDevices.Tests.DeviceTests
{
    public abstract class WhenOpeningStream : WhenTestingDevice
    {
        protected Stream OpenedStream { get; set; }
        protected Exception ThrownException { get; set; }
        protected IStream UnderlyingStream { get; set; }
        protected Stream DeviceStream { get; set; }

        protected override void GivenThat()
        {
            base.GivenThat();

            PortableDeviceHelper.Stub(
                helper => helper.GetChildObjectIds(PortableDeviceContent, PortableDeviceConstants.WPD_DEVICE_OBJECT_ID))
                .Return(new[] { "InternalStorageID" });

            PortableDeviceHelper.Stub(
                helper => helper.GetChildObjectIds(PortableDeviceContent, "InternalStorageID"))
                .Return(new[] { "fooId", "xId" });

            PortableDeviceHelper
                .Stub(propertyHelper => propertyHelper.GetObjectFileName(
                    PortableDeviceContent,
                    "InternalStorageID"))
                .Return("Internal Storage");

            PortableDeviceHelper
                .Stub(propertyHelper => propertyHelper.GetObjectFileName(
                    PortableDeviceContent,
                    "xId"))
                .Return("x");

            PortableDeviceHelper
                .Stub(propertyHelper => propertyHelper.GetObjectFileName(
                    PortableDeviceContent,
                    "fooId"))
                .Return("foo.mp3");

            UnderlyingStream = GenerateMock<IStream>();

            DeviceStream = GeneratePartialMock<Stream>();
        }

        protected override void When()
        {
            try
            {
                DoWhen();
            }
            catch (Exception exception)
            {
                ThrownException = exception;
            }
        }

        protected abstract void DoWhen();
    }
}