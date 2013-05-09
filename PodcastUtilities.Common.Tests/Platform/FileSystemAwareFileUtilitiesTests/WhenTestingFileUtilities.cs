using System.IO;
using PodcastUtilities.Common.Platform;
using PodcastUtilities.PortableDevices;

namespace PodcastUtilities.Common.Tests.Platform.FileSystemAwareFileUtilitiesTests
{
    public abstract class WhenTestingFileUtilities
        : WhenTestingBehaviour
    {
        protected IFileUtilities RegularFileUtilities { get; set; }
        protected IDeviceManager DeviceManager { get; set; }
        protected IStreamHelper StreamHelper { get; set; }

        protected Stream SourceStream { get; set; }
        protected Stream DestinationStream { get; set; }

        protected IDevice Device { get; set; }

        protected FileSystemAwareFileUtilities Utilities { get; set; }

        protected override void GivenThat()
        {
            base.GivenThat();

            RegularFileUtilities = GenerateMock<IFileUtilities>();
            DeviceManager = GenerateMock<IDeviceManager>();
            StreamHelper = GenerateMock<IStreamHelper>();

            SourceStream = GeneratePartialMock<Stream>();
            DestinationStream = GeneratePartialMock<Stream>();

            Device = GenerateMock<IDevice>();

            Utilities = new FileSystemAwareFileUtilities(RegularFileUtilities, DeviceManager, StreamHelper);
        }
    }
}