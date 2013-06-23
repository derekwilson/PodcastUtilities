using PodcastUtilities.Common.Platform.Mtp;
using PodcastUtilities.PortableDevices;

namespace PodcastUtilities.Common.Platform
{
    /// <summary>
    /// Provides the right type of DriveInfo depending on whether the path is MTP or not
    /// </summary>
    public class FileSystemAwareDriveInfoProvider : IDriveInfoProvider
    {
        private readonly IDeviceManager _deviceManager;

        ///<summary>
        /// Constructs the directory info provider
        ///</summary>
        ///<param name="deviceManager"></param>
        public FileSystemAwareDriveInfoProvider(IDeviceManager deviceManager)
        {
            _deviceManager = deviceManager;
        }

        /// <summary>
        /// create an abstract drive info object
        /// </summary>
        /// <param name="path">name of the drive/path</param>
        /// <returns>an abstrcat object</returns>
        public IDriveInfo GetDriveInfoForPath(string path)
        {
            if (MtpPath.IsMtpPath(path))
            {
                return new MtpDriveInfoProvider(_deviceManager).GetDriveInfoForPath(path);
            }

            return new SystemDriveInfoProvider().GetDriveInfoForPath(path);
        }
    }
}