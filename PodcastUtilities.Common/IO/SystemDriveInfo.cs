using System;
using System.IO;

namespace PodcastUtilities.Common.IO
{
	internal class SystemDriveInfo : IDriveInfo
	{
		private readonly DriveInfo _driveInfo;

		public SystemDriveInfo(string driveName)
		{
			_driveInfo = new DriveInfo(driveName);
		}

		public long AvailableFreeSpace
		{
			get { return _driveInfo.AvailableFreeSpace; }
		}

		public string Name
		{
			get { return _driveInfo.Name; }
		}
	}
}