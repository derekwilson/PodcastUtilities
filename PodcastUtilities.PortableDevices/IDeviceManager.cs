using System.Collections.Generic;

namespace PodcastUtilities.PortableDevices
{
    public interface IDeviceManager
    {
        IEnumerable<IDevice> GetAllDevices();
        IDevice GetDevice(string deviceName);
    }
}