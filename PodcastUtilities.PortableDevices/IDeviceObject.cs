using System.Collections.Generic;

namespace PodcastUtilities.PortableDevices
{
    public interface IDeviceObject
    {
        IEnumerable<IDeviceObject> GetFolders(string pattern);
        IEnumerable<IDeviceObject> GetFiles(string pattern);
    }
}