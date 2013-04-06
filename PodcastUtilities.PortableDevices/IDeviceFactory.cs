namespace PodcastUtilities.PortableDevices
{
    public interface IDeviceFactory
    {
        IDevice CreateDevice(string id);
    }
}