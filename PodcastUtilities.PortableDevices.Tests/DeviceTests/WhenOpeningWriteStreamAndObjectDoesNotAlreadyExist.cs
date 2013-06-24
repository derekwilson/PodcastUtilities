using NUnit.Framework;
using Rhino.Mocks;

namespace PodcastUtilities.PortableDevices.Tests.DeviceTests
{
    public class WhenOpeningWriteStreamAndObjectDoesNotAlreadyExist : WhenOpeningStream
    {
        protected override void GivenThat()
        {
            base.GivenThat();

            PortableDeviceHelper.Stub(
                helper => helper.CreateFolderObject(PortableDeviceContent, "xId", "y"))
                .Return("yId");

            PortableDeviceHelper.Stub(
                helper => helper.GetChildObjectIds(PortableDeviceContent, "xId"))
                .Return(new string[0]);

            PortableDeviceHelper.Stub(
                helper => helper.CreateResourceStream(PortableDeviceContent, "yId", "new.mp3", 4567))
                .Return(UnderlyingStream);

            DeviceStreamFactory.Stub(factory => factory.CreateStream(UnderlyingStream))
                .Return(DeviceStream);
        }

        protected override void DoWhen()
        {
            OpenedStream = Device.OpenWrite(@"Internal Storage\x\y\new.mp3", 4567, false);
        }

        [Test]
        public void ItShouldCreateParentFolderObject()
        {
            PortableDeviceHelper.AssertWasCalled(
                helper => helper.CreateFolderObject(PortableDeviceContent, "xId", "y"));
        }

        [Test]
        public void ItShouldReturnDeviceStreamWrappingCreatedUnderlyingResourceStream()
        {
            Assert.That(OpenedStream, Is.SameAs(DeviceStream));
        }
    }
}