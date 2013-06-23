using PodcastUtilities.PortableDevices;

namespace PodcastUtilities.Common.Platform.Mtp
{
    ///<summary>
    /// Provide Drive info like funcionality for MTP "drives"
    ///</summary>
    public class DriveInfo
        : IDriveInfo
    {
        private readonly IDevice _device;
        private readonly IDeviceObject _storageObject;

        internal DriveInfo(IDevice device, IDeviceObject storageObject)
        {
            _device = device;
            _storageObject = storageObject;
        }


        /// <summary>
        /// the free space in bytes
        /// </summary>
        public long AvailableFreeSpace
        {
            get { return _storageObject.AvailableFreeSpace; }
        }

        /// <summary>
        /// the name of the drive
        /// </summary>
        public string Name
        {
            get { return _storageObject.Name; }
        }
    }
}