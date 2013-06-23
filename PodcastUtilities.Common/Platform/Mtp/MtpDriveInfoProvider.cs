using System;
using System.IO;
using PodcastUtilities.PortableDevices;

namespace PodcastUtilities.Common.Platform.Mtp
{
    ///<summary>
    /// MTP implementation of drive info provider
    ///</summary>
    public class MtpDriveInfoProvider
        : IDriveInfoProvider
    {
        private readonly IDeviceManager _deviceManager;

        ///<summary>
        /// Constructs the drive info provider
        ///</summary>
        ///<param name="deviceManager"></param>
        public MtpDriveInfoProvider(IDeviceManager deviceManager)
        {
            _deviceManager = deviceManager;
        }

        /// <summary>
        /// create an abstract drive info object
        /// </summary>
        /// <param name="path">name of the drive</param>
        /// <returns>an abstrcat object</returns>
        public IDriveInfo GetDriveInfoForPath(string path)
        {
            var pathInfo = MtpPath.GetPathInfo(path);

            var device = _deviceManager.GetDevice(pathInfo.DeviceName);

            if (device == null)
            {
                throw new DriveNotFoundException(String.Format("Device [{0}] not found", pathInfo.DeviceName));
            }

            var storageObject = device.GetRootStorageObjectFromPath(pathInfo.RelativePathOnDevice);
            if (storageObject == null)
            {
                throw new DriveNotFoundException(
                    String.Format(
                        "No storage object found: Device [{0}], path [{1}]", 
                        pathInfo.DeviceName,
                        pathInfo.RelativePathOnDevice));
            }

            return new DriveInfo(device, storageObject);
        }
    }
}