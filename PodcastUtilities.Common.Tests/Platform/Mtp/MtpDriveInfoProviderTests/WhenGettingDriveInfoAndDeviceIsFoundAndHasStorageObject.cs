using NUnit.Framework;
using PodcastUtilities.Common.Platform.Mtp;
using PodcastUtilities.PortableDevices;
using Rhino.Mocks;

namespace PodcastUtilities.Common.Tests.Platform.Mtp.MtpDriveInfoProviderTests
{
    public class WhenGettingDriveInfoAndDeviceIsFoundAndHasStorageObject : WhenGettingDriveInfoAndDeviceIsFound
    {
        protected override void GivenThat()
        {
            base.GivenThat();

            var storageObject = GenerateMock<IDeviceObject>();
            storageObject.Stub(storage => storage.Name)
                .Return("storage");

            Device.Stub(device => device.GetRootStorageObjectFromPath(@"storage\b\c"))
                .Return(storageObject);
        }

        protected override void When()
        {
            DriveInfo = DriveInfoProvider.GetDriveInfoForPath(@"mtp:\test device\storage\b\c");
        }

        [Test]
        public void ItShouldCreateCorrectTypeOfDriveInfo()
        {
            Assert.That(DriveInfo, Is.InstanceOf<DriveInfo>());
        }

        [Test]
        public void ItShouldCreateDriveInfoCorrectly()
        {
            Assert.That(DriveInfo.Name, Is.EqualTo(@"storage"));
        }
    }
}