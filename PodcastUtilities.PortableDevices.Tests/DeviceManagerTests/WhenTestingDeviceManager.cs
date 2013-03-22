using PodcastUtilities.Common.Tests;
using PortableDeviceApiLib;

namespace PodcastUtilities.PortableDevices.Tests.DeviceManagerTests
{
    public abstract class WhenTestingDeviceManager
        : WhenTestingBehaviour
    {
        protected DeviceManager DeviceManager { get; set; }
        protected IPortableDeviceManager PortableDeviceManager { get; set; }
        protected IPortableDeviceFactory PortableDeviceFactory { get; set; }
        protected IPortableDevice PortableDevice { get; set; }

        protected override void GivenThat()
        {
            base.GivenThat();

            // Using a generated stub doesn't work as the stub seems to get confused with
            // all the Marshalling gubbins on the interface.
            PortableDeviceManager = new MockPortableDeviceManager();

            PortableDevice = GenerateMock<IPortableDevice>();
            PortableDeviceFactory = GenerateMock<IPortableDeviceFactory>();

            DeviceManager = new DeviceManager(PortableDeviceManager, PortableDeviceFactory);
        }
    }
}