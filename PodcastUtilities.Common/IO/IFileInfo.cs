using System;

namespace PodcastUtilities.Common.IO
{
	/// <summary>
    /// methods to query files in the physical file system and abstract away the file system from the main body of code
	/// </summary>
    public interface IFileInfo
	{
        /// <summary>
        /// the name of the file eg. file.ext
        /// </summary>
        string Name { get; }
        /// <summary>
        /// the full pathname of the object eg. c:\media\file.ext
        /// </summary>
		string FullName { get; }
        /// <summary>
        /// date time the file was created
        /// </summary>
        DateTime CreationTime { get; }
	}
}