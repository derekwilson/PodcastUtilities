using NUnit.Framework;
using PodcastUtilities.PortableDevices;
using Rhino.Mocks;

namespace PodcastUtilities.Common.Tests.Platform.FileSystemAwareFileUtilitiesTests.FileExists
{
    public abstract class WhenPathIsMtp
        : WhenTestingFileUtilities
    {
        protected bool FileExists { get; set; }

        protected override void GivenThat()
        {
            base.GivenThat();

            DeviceManager.Stub(manager => manager.GetDevice("my device"))
                .Return(Device);
        }

        protected override void When()
        {
            FileExists = Utilities.FileExists(@"mtp:\my device\foo\bar.abc");
        }
    }

    public class WhenPathIsMtpAndDeviceObjectIsNull
        : WhenPathIsMtp
    {
        protected override void GivenThat()
        {
            base.GivenThat();

            Device.Stub(device => device.GetObjectFromPath(@"foo\bar.abc"))
                .Return(null);
        }

        [Test]
        public void ItShouldReturnFalse()
        {
            Assert.That(FileExists, Is.False);
        }
    }

    public class WhenPathIsMtpAndDeviceObjectIsNotNull
        : WhenPathIsMtp
    {
        protected override void GivenThat()
        {
            base.GivenThat();

            Device.Stub(device => device.GetObjectFromPath(@"foo\bar.abc"))
                .Return(GenerateMock<IDeviceObject>());
        }

        [Test]
        public void ItShouldReturnTrue()
        {
            Assert.That(FileExists, Is.True);
        }
    }
}