using System.IO;

namespace PodcastUtilities.Common.Platform
{
    /// <summary>
    /// methods to query file system drives in the physical file system and abstract away the file system from the main body of code
    /// </summary>
    internal class SystemDriveInfo : IDriveInfo
	{
		private readonly DriveInfo _driveInfo;

		/// <summary>
		/// construct an object from the specified name
		/// </summary>
		/// <param name="driveName"></param>
        public SystemDriveInfo(string driveName)
		{
			_driveInfo = new DriveInfo(driveName);
		}

        /// <summary>
        /// the free space in bytes
        /// </summary>
        public long AvailableFreeSpace
		{
			get { return _driveInfo.AvailableFreeSpace; }
		}

        /// <summary>
        /// the name of the drive
        /// </summary>
        public string Name
		{
			get { return _driveInfo.Name; }
		}
	}
}