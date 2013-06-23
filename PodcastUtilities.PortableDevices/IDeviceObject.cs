using System.Collections.Generic;

namespace PodcastUtilities.PortableDevices
{
    public interface IDeviceObject
    {
        string Id { get; }
        string Name { get; }

        // Only relevant for storage objects - so maybe shouldn't be here...
        long AvailableFreeSpace { get; }

        IEnumerable<IDeviceObject> GetFolders(string pattern);
        IEnumerable<IDeviceObject> GetFiles(string pattern);
    }
}