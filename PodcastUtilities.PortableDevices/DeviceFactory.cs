using PortableDeviceApiLib;

namespace PodcastUtilities.PortableDevices
{
    public class DeviceFactory : IDeviceFactory
    {
        private readonly IPortableDeviceManager _portableDeviceManager;
        private readonly IPortableDeviceFactory _portableDeviceFactory;

        internal DeviceFactory(
            IPortableDeviceManager portableDeviceManager)
        {
            _portableDeviceManager = portableDeviceManager;
            _portableDeviceFactory = new PortableDeviceFactory();
        }

        public IDevice CreateDevice(string id)
        {
            return new Device(_portableDeviceManager, _portableDeviceFactory, id);
        }
    }
}