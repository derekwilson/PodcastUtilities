using NUnit.Framework;
using PodcastUtilities.Common.Platform;
using PodcastUtilities.Common.Platform.Mtp;
using PodcastUtilities.PortableDevices;
using Rhino.Mocks;

namespace PodcastUtilities.Common.Tests.Platform.Mtp.MtpDirectoryInfoProviderTests
{
    public class WhenGettingDirectoryInfoAndDeviceIsFound : WhenTestingMtpDirectoryInfoProvider
    {
        protected IDevice Device { get; set; }
        protected IDirectoryInfo DirectoryInfo { get; set; }

        protected override void GivenThat()
        {
            base.GivenThat();

            Device = GenerateMock<IDevice>();
            Device.Stub(device => device.Name)
                .Return("test device");

            DeviceManager.Stub(manager => manager.GetDevice("test device"))
                .Return(Device);
        }

        protected override void When()
        {
            DirectoryInfo = DirectoryInfoProvider.GetDirectoryInfo(@"mtp:\test device\a\b\c");
        }

        [Test]
        public void ItShouldCreateCorrectTypeOfDirectoryInfo()
        {
            Assert.That(DirectoryInfo, Is.InstanceOf<DirectoryInfo>());
        }

        [Test]
        public void ItShouldCreateDirectoryInfoCorrectly()
        {
            Assert.That(DirectoryInfo.FullName, Is.EqualTo(@"MTP:\test device\a\b\c"));
        }
    }
}