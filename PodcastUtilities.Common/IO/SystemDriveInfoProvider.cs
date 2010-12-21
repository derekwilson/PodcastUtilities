namespace PodcastUtilities.Common.IO
{
	public class SystemDriveInfoProvider : IDriveInfoProvider
	{
		public IDriveInfo GetDriveInfo(string driveName)
		{
			return new SystemDriveInfo(driveName);
		}
	}
}