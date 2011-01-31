using System;

namespace PodcastUtilities.Common.IO
{
	public interface IDriveInfo
	{
		long AvailableFreeSpace { get; }

		string Name { get; }
	}
}
