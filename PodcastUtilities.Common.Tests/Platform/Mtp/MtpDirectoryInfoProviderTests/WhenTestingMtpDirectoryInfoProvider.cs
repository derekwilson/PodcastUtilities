using PodcastUtilities.Common.Platform.Mtp;
using PodcastUtilities.PortableDevices;

namespace PodcastUtilities.Common.Tests.Platform.Mtp.MtpDirectoryInfoProviderTests
{
    public abstract class WhenTestingMtpDirectoryInfoProvider
        : WhenTestingBehaviour
    {
        protected IDeviceManager DeviceManager { get; set; }
        protected MtpDirectoryInfoProvider DirectoryInfoProvider { get; set; }

        protected override void GivenThat()
        {
            base.GivenThat();

            DeviceManager = GenerateMock<IDeviceManager>();

            DirectoryInfoProvider = new MtpDirectoryInfoProvider(DeviceManager);
        }
    }
}