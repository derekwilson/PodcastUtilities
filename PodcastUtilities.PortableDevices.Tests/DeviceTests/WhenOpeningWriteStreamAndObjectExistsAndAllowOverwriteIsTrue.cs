using NUnit.Framework;
using Rhino.Mocks;

namespace PodcastUtilities.PortableDevices.Tests.DeviceTests
{
    public class WhenOpeningWriteStreamAndObjectExistsAndAllowOverwriteIsTrue : WhenOpeningStream
    {
        protected override void GivenThat()
        {
            base.GivenThat();

            PortableDeviceHelper.Stub(
                helper => helper.CreateResourceStream(PortableDeviceContent, "InternalStorageID", "foo.mp3", 2345))
                .Return(UnderlyingStream);

            DeviceStreamFactory.Stub(factory => factory.CreateStream(UnderlyingStream))
                .Return(DeviceStream);
        }

        protected override void DoWhen()
        {
            OpenedStream = Device.OpenWrite(@"Internal Storage\foo.mp3", 2345, true);
        }

        [Test]
        public void ItShouldDeleteOriginalObject()
        {
            PortableDeviceHelper.AssertWasCalled(helper => helper.DeleteObject(PortableDeviceContent, "fooId"));
        }

        [Test]
        public void ItShouldReturnDeviceStreamWrappingCreatedUnderlyingResourceStream()
        {
            Assert.That(OpenedStream, Is.SameAs(DeviceStream));
        }
    }
}