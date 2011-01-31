using System;

namespace PodcastUtilities.Common.IO
{
	public interface IFileInfo
	{
		string Name { get; }
		string FullName { get; }
		DateTime CreationTime { get; }
	}
}