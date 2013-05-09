using System;
using System.IO;
using PodcastUtilities.PortableDevices;

namespace PodcastUtilities.Common.Platform.Mtp
{
    /// <summary>
    /// MTP implementation of directory info provider
    /// </summary>
    public class MtpDirectoryInfoProvider
        : IDirectoryInfoProvider
    {
        private readonly IDeviceManager _deviceManager;

        ///<summary>
        /// Constructs the directory info provider
        ///</summary>
        ///<param name="deviceManager"></param>
        public MtpDirectoryInfoProvider(IDeviceManager deviceManager)
        {
            _deviceManager = deviceManager;
        }

        /// <summary>
        /// create an abstract directory object
        /// </summary>
        /// <param name="path">full path to the directory</param>
        /// <returns>an abstrcat object</returns>
        public IDirectoryInfo GetDirectoryInfo(string path)
        {
            var pathInfo = MtpPath.GetPathInfo(path);

            var device = _deviceManager.GetDevice(pathInfo.DeviceName);

            if (device == null)
            {
                throw new DirectoryNotFoundException(String.Format("Device [{0}] not found", pathInfo.DeviceName));
            }

            return new DirectoryInfo(device, pathInfo.RelativePathOnDevice);
        }
    }
}