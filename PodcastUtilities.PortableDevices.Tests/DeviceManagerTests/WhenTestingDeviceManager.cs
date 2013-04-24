using PodcastUtilities.Common.Tests;
using PortableDeviceApiLib;
using Rhino.Mocks;

namespace PodcastUtilities.PortableDevices.Tests.DeviceManagerTests
{
    public abstract class WhenTestingDeviceManager
        : WhenTestingBehaviour
    {
        protected DeviceManager DeviceManager { get; set; }
        protected IPortableDeviceManager PortableDeviceManager { get; set; }
        protected IPortableDevice PortableDevice { get; set; }
        protected IDeviceFactory DeviceFactory { get; set; }
        protected IDevice Device1 { get; set; }
        protected IDevice Device2 { get; set; }

        protected override void GivenThat()
        {
            base.GivenThat();

            PortableDeviceManager = new MockPortableDeviceManager();

            PortableDevice = GenerateMock<IPortableDevice>();

            Device1 = GenerateMock<IDevice>();
            Device1.Stub(device => device.Name)
                .Return("Device 1");
            Device1.Stub(device => device.Id)
                .Return("Device_Id_1");

            Device2 = GenerateMock<IDevice>();
            Device2.Stub(device => device.Name)
                .Return("Device 2");
            Device2.Stub(device => device.Id)
                .Return("Device_Id_2");

            DeviceFactory = GenerateMock<IDeviceFactory>();
            DeviceFactory.Stub(factory => factory.CreateDevice("Device_Id_1"))
                .Return(Device1);
            DeviceFactory.Stub(factory => factory.CreateDevice("Device_Id_2"))
                .Return(Device2);

            DeviceManager = new DeviceManager(PortableDeviceManager, DeviceFactory);
        }
    }
}