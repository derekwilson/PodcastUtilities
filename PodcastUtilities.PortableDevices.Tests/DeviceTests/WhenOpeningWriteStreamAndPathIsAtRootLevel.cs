using NUnit.Framework;
using Rhino.Mocks;

namespace PodcastUtilities.PortableDevices.Tests.DeviceTests
{
    public class WhenOpeningWriteStreamAndPathIsAtRootLevel : WhenOpeningStream
    {
        protected override void GivenThat()
        {
            base.GivenThat();

            // Root parent is Device
            PortableDeviceHelper.Stub(
                helper => helper.CreateResourceStream(PortableDeviceContent, PortableDeviceConstants.WPD_DEVICE_OBJECT_ID, "new.mp3", 3456))
                .Return(UnderlyingStream);

            DeviceStreamFactory.Stub(factory => factory.CreateStream(UnderlyingStream))
                .Return(DeviceStream);
        }

        protected override void DoWhen()
        {
            OpenedStream = Device.OpenWrite(@"new.mp3", 3456, false);
        }

        [Test]
        public void ItShouldReturnDeviceStreamWrappingCreatedUnderlyingResourceStream()
        {
            Assert.That(OpenedStream, Is.SameAs(DeviceStream));
        }
    }
}