using PodcastUtilities.Common.Platform;
using PodcastUtilities.PortableDevices;
using Rhino.Mocks;

namespace PodcastUtilities.Common.Tests.Platform.Mtp.MtpDriveInfoProviderTests
{
    public abstract class WhenGettingDriveInfoAndDeviceIsFound : WhenTestingMtpDriveInfoProvider
    {
        protected IDevice Device { get; set; }
        protected IDriveInfo DriveInfo { get; set; }

        protected override void GivenThat()
        {
            base.GivenThat();

            Device = GenerateMock<IDevice>();
            Device.Stub(device => device.Name)
                .Return("test device");

            DeviceManager.Stub(manager => manager.GetDevice("test device"))
                .Return(Device);
        }
    }
}