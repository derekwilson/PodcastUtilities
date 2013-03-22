using System.Collections.Generic;
using System.Linq;
using PortableDeviceApiLib;

namespace PodcastUtilities.PortableDevices
{
    public class DeviceManager : IDeviceManager
    {
        private readonly IPortableDeviceManager _portableDeviceManager;
        private readonly IPortableDeviceFactory _portableDeviceFactory;

        private Dictionary<string, Device> _deviceNameCache;
        private Dictionary<string, Device> _deviceIdCache;

        public DeviceManager()
            : this(new PortableDeviceManagerClass(), new PortableDeviceFactory())
        {
        }

        internal DeviceManager(
            IPortableDeviceManager portableDeviceManager,
            IPortableDeviceFactory portableDeviceFactory)
        {
            _portableDeviceManager = portableDeviceManager;
            _portableDeviceFactory = portableDeviceFactory;
        }

        public IDevice GetDevice(string deviceName)
        {
            EnumerateDevices();

            Device device;
            _deviceNameCache.TryGetValue(deviceName, out device);

            return device;
        }

        private void EnumerateDevices()
        {
            if ((_deviceNameCache != null) && (_deviceIdCache != null))
            {
                return;
            }

            uint deviceCount = 0;
            _portableDeviceManager.GetDevices(null, ref deviceCount);

            var deviceIds = new string[deviceCount];
            _portableDeviceManager.GetDevices(deviceIds, ref deviceCount);

            var devices = deviceIds.Select(CreateDevice).ToList();

            _deviceNameCache = devices.ToDictionary(device => device.Name);
            _deviceIdCache = devices.ToDictionary(device => device.Id);
        }

        private Device CreateDevice(string id)
        {
            return new Device(_portableDeviceManager, _portableDeviceFactory, id);
        }
    }
}