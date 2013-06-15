using PodcastUtilities.Common.Tests;
using PortableDeviceApiLib;
using Rhino.Mocks;

namespace PodcastUtilities.PortableDevices.Tests.DeviceTests
{
    public abstract class WhenTestingDevice
        : WhenTestingBehaviour
    {
        protected IPortableDeviceManager PortableDeviceManager { get; set; }
        protected IPortableDeviceFactory PortableDeviceFactory { get; set; }
        protected IPortableDeviceHelper PortableDeviceHelper { get; set; }
        protected IDeviceStreamFactory DeviceStreamFactory { get; set; }
        protected IPortableDevice PortableDevice { get; set; }
        protected IPortableDeviceContent PortableDeviceContent { get; set; }

        protected Device Device { get; set; }

        protected override void GivenThat()
        {
            base.GivenThat();

            PortableDeviceManager = new MockPortableDeviceManager();

            PortableDeviceFactory = GenerateMock<IPortableDeviceFactory>();
            PortableDeviceHelper = GenerateMock<IPortableDeviceHelper>();
            DeviceStreamFactory = GenerateMock<IDeviceStreamFactory>();

            PortableDevice = GenerateMock<IPortableDevice>();
            PortableDeviceContent = GenerateMock<IPortableDeviceContent>();

            PortableDeviceFactory.Stub(factory => factory.Create("ABC123"))
                .Return(PortableDevice);

            PortableDevice.Stub(device => device.Content(out Arg<IPortableDeviceContent>.Out(PortableDeviceContent).Dummy));

            Device = new Device(PortableDeviceManager, PortableDeviceFactory, PortableDeviceHelper, DeviceStreamFactory, "ABC123");
        }
    }
}