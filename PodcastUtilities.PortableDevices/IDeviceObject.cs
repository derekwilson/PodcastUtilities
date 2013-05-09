using System.Collections.Generic;

namespace PodcastUtilities.PortableDevices
{
    public interface IDeviceObject
    {
        string Id { get; }
        string Name { get; }
        IEnumerable<IDeviceObject> GetFolders(string pattern);
        IEnumerable<IDeviceObject> GetFiles(string pattern);
    }
}