using System;
using System.IO;
using PodcastUtilities.Common.Platform.Mtp;
using PodcastUtilities.PortableDevices;
using DirectoryInfo = PodcastUtilities.Common.Platform.Mtp.DirectoryInfo;
using FileInfo = PodcastUtilities.Common.Platform.Mtp.FileInfo;

namespace PodcastUtilities.Common.Platform
{
    /// <summary>
    /// Provides the correct kind of file info object
    /// </summary>
    public class FileSystemAwareFileInfoProvider : IFileInfoProvider
    {
        private readonly IDeviceManager _deviceManager;

        ///<summary>
        /// Constructs the directory info provider
        ///</summary>
        ///<param name="deviceManager"></param>
        public FileSystemAwareFileInfoProvider(IDeviceManager deviceManager)
        {
            _deviceManager = deviceManager;
        }

        /// <summary>
        /// create an abstract file info object
        /// </summary>
        /// <param name="path">full path to the file</param>
        /// <returns>the file info</returns>
        public IFileInfo GetFileInfo(string path)
        {
            if (MtpPath.IsMtpPath(path))
            {
                var pathInfo = MtpPath.GetPathInfo(path);

                var device = _deviceManager.GetDevice(pathInfo.DeviceName);

                if (device == null)
                {
                    throw new FileNotFoundException(String.Format("Device [{0}] not found", pathInfo.DeviceName));
                }

                return new FileInfo(device, pathInfo.RelativePathOnDevice);
            }

            return new SystemFileInfo(new System.IO.FileInfo(path));
        }
    }
}