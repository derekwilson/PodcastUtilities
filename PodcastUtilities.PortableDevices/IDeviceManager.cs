namespace PodcastUtilities.PortableDevices
{
    public interface IDeviceManager
    {
        IDevice GetDevice(string deviceName);
    }
}