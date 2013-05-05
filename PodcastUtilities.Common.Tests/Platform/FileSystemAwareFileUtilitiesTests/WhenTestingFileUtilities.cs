using PodcastUtilities.Common.Platform;
using PodcastUtilities.PortableDevices;

namespace PodcastUtilities.Common.Tests.Platform.FileSystemAwareFileUtilitiesTests
{
    public abstract class WhenTestingFileUtilities
        : WhenTestingBehaviour
    {
        protected IFileUtilities RegularFileUtilities { get; set; }
        protected IDeviceManager DeviceManager { get; set; }

        protected FileSystemAwareFileUtilities Utilities { get; set; }

        protected override void GivenThat()
        {
            base.GivenThat();

            RegularFileUtilities = GenerateMock<IFileUtilities>();
            DeviceManager = GenerateMock<IDeviceManager>();

            Utilities = new FileSystemAwareFileUtilities(RegularFileUtilities, DeviceManager);
        }
    }
}