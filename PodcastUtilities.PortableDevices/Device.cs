using System;
using PortableDeviceApiLib;

namespace PodcastUtilities.PortableDevices
{
    public class Device : IDevice
    {
        private readonly IPortableDeviceManager _portableDeviceManager;
        private readonly IPortableDeviceFactory _portableDeviceFactory;
        private IPortableDevice _portableDevice;
        private string _name;

        internal Device(
            IPortableDeviceManager portableDeviceManager, 
            IPortableDeviceFactory portableDeviceFactory, 
            string id)
        {
            _portableDeviceManager = portableDeviceManager;
            _portableDeviceFactory = portableDeviceFactory;
            Id = id;
            _portableDevice = OpenDevice(id);
        }

        public string Id { get; private set; }

        public string Name
        {
            get { return _name ?? (_name = GetDeviceName()); }
        }

        public IDeviceObject GetObjectFromPath(string path)
        {
            throw new NotImplementedException();
        }

        private IPortableDevice OpenDevice(string id)
        {
            throw new NotImplementedException();
        }

        private string GetDeviceName()
        {
            return "";
        }
    }
}