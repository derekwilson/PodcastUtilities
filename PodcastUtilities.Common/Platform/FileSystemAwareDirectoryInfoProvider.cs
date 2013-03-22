using PodcastUtilities.Common.Platform.Mtp;
using PodcastUtilities.PortableDevices;

namespace PodcastUtilities.Common.Platform
{
    ///<summary>
    /// Knows which type of directory info provider to use based on the path
    ///</summary>
    public class FileSystemAwareDirectoryInfoProvider
        : IDirectoryInfoProvider
    {
        private readonly IDeviceManager _deviceManager;

        ///<summary>
        /// Constructs the directory info provider
        ///</summary>
        ///<param name="deviceManager"></param>
        public FileSystemAwareDirectoryInfoProvider(IDeviceManager deviceManager)
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
            if (MtpPath.IsMtpPath(path))
            {
                return new MtpDirectoryInfoProvider(_deviceManager).GetDirectoryInfo(path);
            }

            return new SystemDirectoryInfoProvider().GetDirectoryInfo(path);
        }
    }
}