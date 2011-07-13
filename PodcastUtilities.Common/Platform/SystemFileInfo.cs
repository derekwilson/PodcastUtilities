using System;
using System.IO;

namespace PodcastUtilities.Common.Platform
{
    /// <summary>
    /// methods to query files in the physical file system and abstract away the file system from the main body of code
    /// </summary>
    class SystemFileInfo : IFileInfo
	{
		private readonly FileInfo _fileInfo;

		/// <summary>
		/// construct from a file pathname as a string
		/// </summary>
		/// <param name="path"></param>
        public SystemFileInfo(string path)
			: this(new FileInfo(path))
		{
		}

        /// <summary>
        /// construct from anothe absract object
        /// </summary>
        /// <param name="fileInfo">object to construct from</param>
        public SystemFileInfo(FileInfo fileInfo)
		{
			_fileInfo = fileInfo;
		}

		#region Implementation of IFileInfo

        /// <summary>
        /// the name of the file eg. file.ext
        /// </summary>
        public string Name
		{
			get { return _fileInfo.Name; }
		}

        /// <summary>
        /// the full pathname of the object eg. c:\media\file.ext
        /// </summary>
        public string FullName
		{
			get { return _fileInfo.FullName; }
		}

        /// <summary>
        /// date time the file was created
        /// </summary>
        public DateTime CreationTime
		{
			get { return _fileInfo.CreationTime; }
		}

		#endregion
	}
}