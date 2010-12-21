using System;

namespace PodcastUtilities.Common.IO
{
	public interface IDirectoryInfo
	{
		IDirectoryInfo Root { get; }

		string FullName { get; }
	}
}