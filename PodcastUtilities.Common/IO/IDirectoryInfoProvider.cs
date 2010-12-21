using System;

namespace PodcastUtilities.Common.IO
{
	public interface IDirectoryInfoProvider
	{
		IDirectoryInfo GetDirectoryInfo(string path);
	}
}