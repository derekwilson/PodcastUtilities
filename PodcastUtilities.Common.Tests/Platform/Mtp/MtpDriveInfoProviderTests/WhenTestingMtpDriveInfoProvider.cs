using PodcastUtilities.Common.Platform.Mtp;
using PodcastUtilities.PortableDevices;

namespace PodcastUtilities.Common.Tests.Platform.Mtp.MtpDriveInfoProviderTests
{
    public abstract class WhenTestingMtpDriveInfoProvider
        : WhenTestingBehaviour
    {
        protected IDeviceManager DeviceManager { get; set; }
        protected MtpDriveInfoProvider DriveInfoProvider { get; set; }

        protected override void GivenThat()
        {
            base.GivenThat();

            DeviceManager = GenerateMock<IDeviceManager>();

            DriveInfoProvider = new MtpDriveInfoProvider(DeviceManager);
        }
    }
}