using System;
using System.IO;
using System.Linq;
using PodcastUtilities.PortableDevices;

namespace PodcastUtilities.Common.Platform.Mtp
{
    ///<summary>
    /// Implementation of abstract directory info for MTP "directories" (parent objects)
    ///</summary>
    public class DirectoryInfo
        : IDirectoryInfo
    {
        private readonly IDevice _device;
        private IDeviceObject _deviceObject;
        private readonly string _path;

        internal DirectoryInfo(IDevice device, string pathToObject)
            : this(device, null, pathToObject)
        {
        }

        internal DirectoryInfo(IDevice device, IDeviceObject deviceObject, string pathToObject)
        {
            _device = device;
            _deviceObject = deviceObject;
            _path = pathToObject;
        }

        /// <summary>
        /// the full pathname of the directory
        /// </summary>
        public string FullName
        {
            get { return MtpPath.MakeFullPath(MtpPath.Combine(_device.Name, _path)); }
        }

        /// <summary>
        /// true if it exists
        /// </summary>
        public bool Exists
        {
            get { return DeviceObject != null; }
        }

        private IDeviceObject DeviceObject
        {
            get
            {
                return _deviceObject ?? (_deviceObject = _device.GetObjectFromPath(_path));
            }
        }

        /// <summary>
        /// gets an abstract collection of files that are contained by by the directory
        /// </summary>
        /// <param name="pattern">a search patter for example *.mp3</param>
        /// <returns>a collection of abstracted files</returns>
        public IFileInfo[] GetFiles(string pattern)
        {
            ThrowIfObjectNotFound();

//            var childFileObjects = DeviceObject.GetFiles(pattern);
//
//            return childFileObjects.Select(folder => new FileInfo(_device, folder, "")).ToArray();

            return new IFileInfo[0];
        }

        /// <summary>
        /// gets an abstract collection of directories that are contained by the directory
        /// </summary>
        /// <param name="pattern">a search patter for example *.*</param>
        /// <returns>a collection of abstracted files</returns>
        public IDirectoryInfo[] GetDirectories(string pattern)
        {
            ThrowIfObjectNotFound();

            var childFolderObjects = DeviceObject.GetFolders(pattern);

            return childFolderObjects.Select(folder => new DirectoryInfo(_device, folder, "")).ToArray();
        }

        /// <summary>
        /// create the directory in the file system
        /// </summary>
        public void Create()
        {
            if (Exists)
            {
                return;
            }
        }

        private void ThrowIfObjectNotFound()
        {
            if (DeviceObject == null)
            {
                throw new DirectoryNotFoundException(String.Format("Path [{0}] not found on device [{1}]", _path, _device.Name));
            }
        }
    }
}