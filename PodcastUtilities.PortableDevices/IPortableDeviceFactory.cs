using PortableDeviceApiLib;

namespace PodcastUtilities.PortableDevices
{
    internal interface IPortableDeviceFactory
    {
        IPortableDevice Create(string deviceId);
    }
}