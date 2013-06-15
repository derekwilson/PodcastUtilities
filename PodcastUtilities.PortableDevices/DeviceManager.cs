using System.Collections.Generic;
using System.Linq;
using PortableDeviceApiLib;

namespace PodcastUtilities.PortableDevices
{
    public class DeviceManager : IDeviceManager
    {
        private readonly IPortableDeviceManager _portableDeviceManager;
        private readonly IDeviceFactory _deviceFactory;

        private Dictionary<string, IDevice> _deviceNameCache;
        private Dictionary<string, IDevice> _deviceIdCache;

        public DeviceManager()
            : this(new PortableDeviceManagerClass())
        {
        }

        internal DeviceManager(
            IPortableDeviceManager portableDeviceManager)
            : this(portableDeviceManager, new DeviceFactory(portableDeviceManager))
        {
        }

        internal DeviceManager(
            IPortableDeviceManager portableDeviceManager,
            IDeviceFactory deviceFactory)
        {
            _portableDeviceManager = portableDeviceManager;
            _deviceFactory = deviceFactory;
        }

        public IDevice GetDevice(string deviceName)
        {
            EnumerateDevices();

            IDevice device;
            _deviceNameCache.TryGetValue(deviceName, out device);

            return device;
        }

        public IEnumerable<IDevice> GetAllDevices()
        {
            EnumerateDevices();
            return _deviceNameCache.Values.AsEnumerable();
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

        private IDevice CreateDevice(string id)
        {
            return _deviceFactory.CreateDevice(id);
        }
    }
}