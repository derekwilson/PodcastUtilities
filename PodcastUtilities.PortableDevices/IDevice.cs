using System.IO;

namespace PodcastUtilities.PortableDevices
{
    public interface IDevice
    {
        string Id { get; }
        string Name { get; }

        IDeviceObject GetObjectFromPath(string path);
        IDeviceObject GetRootStorageObjectFromPath(string path);
        void Delete(string path);
        Stream OpenRead(string path);
        Stream OpenWrite(string path, long length, bool allowOverwrite);
        void CreateFolderObjectFromPath(string path);
    }
}