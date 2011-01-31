using System;

namespace PodcastUtilities.Common.IO
{
	public interface IDriveInfoProvider
	{
		IDriveInfo GetDriveInfo(string driveName);
	}
}
