#region License
// FreeBSD License
// Copyright (c) 2010 - 2013, Andrew Trevarrow and Derek Wilson
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
// 
// Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
// 
// Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED 
// WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A 
// PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR
// ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED
// TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING 
// NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
// POSSIBILITY OF SUCH DAMAGE.
#endregion
using System;
using System.IO;
using PodcastUtilities.Common.Platform.Mtp;
using PodcastUtilities.PortableDevices;

namespace PodcastUtilities.Common.Platform
{
    /// <summary>
    /// Utility methods to manipulate files in the physical file system.
    /// This class abstracts away the file system from the main body of code - it also 
    /// copes with MTP file systems.
    /// </summary>
    public class FileSystemAwareFileUtilities : IFileUtilities
    {
        private readonly IFileUtilities _fileUtilities;
        private readonly IDeviceManager _deviceManager;
        private readonly IStreamHelper _streamHelper;
        private readonly IFileInfoProvider _fileInfoProvider;

        ///<summary>
        /// Construct the object
        ///</summary>
        public FileSystemAwareFileUtilities(
            IDeviceManager deviceManager, 
            IStreamHelper streamHelper,
            IFileInfoProvider fileInfoProvider)
            : this(new FileUtilities(), deviceManager, streamHelper, fileInfoProvider)
        {
            
        }

        // dont make this one public or you will get a recursive error in the IoC container for .NET FULL
        internal FileSystemAwareFileUtilities(
            IFileUtilities fileUtilities,
            IDeviceManager deviceManager,
            IStreamHelper streamHelper,
            IFileInfoProvider fileInfoProvider)
        {
            _fileUtilities = fileUtilities;
            _deviceManager = deviceManager;
            _streamHelper = streamHelper;
            _fileInfoProvider = fileInfoProvider;
        }

        /// <summary>
        /// check if a file exists
        /// </summary>
        /// <param name="path">pathname to check</param>
        /// <returns>true if the file exists</returns>
        public bool FileExists(string path)
        {
            var pathInfo = MtpPath.GetPathInfo(path);

            if (!pathInfo.IsMtpPath)
            {
                return _fileUtilities.FileExists(path);
            }

            var device = _deviceManager.GetDevice(pathInfo.DeviceName);
            if (device == null)
            {
                return false;
            }

            return (device.GetObjectFromPath(pathInfo.RelativePathOnDevice) != null);
        }

        /// <summary>
        /// rename / move a file
        /// </summary>
        /// <param name="sourceFileName">source pathname</param>
        /// <param name="destinationFileName">destination pathname</param>
        public void FileRename(string sourceFileName, string destinationFileName)
        {
            FileRename(sourceFileName, destinationFileName, false);
        }

        /// <summary>
        /// rename / move a file
        /// </summary>
        /// <param name="sourceFileName">source pathname</param>
        /// <param name="destinationFileName">destination pathname</param>
        /// <param name="allowOverwrite">set to true to overwrite an existing destination file</param>
        public void FileRename(string sourceFileName, string destinationFileName, bool allowOverwrite)
        {
            if (MtpPath.IsMtpPath(sourceFileName) || MtpPath.IsMtpPath(destinationFileName))
            {
                throw new NotImplementedException();
            }

            _fileUtilities.FileRename(sourceFileName, destinationFileName, allowOverwrite);
        }

        /// <summary>
        /// copy a file - will not overwrite an existing file
        /// the containing folder will be created if it does not exist
        /// </summary>
        /// <param name="sourceFileName">source pathname</param>
        /// <param name="destinationFileName">destination pathname</param>
        public void FileCopy(string sourceFileName, string destinationFileName)
        {
            FileCopy(sourceFileName, destinationFileName, false);
        }

        /// <summary>
        /// copy a file - the containing folder will be created if it does not exist
        /// </summary>
        /// <param name="sourceFileName">source pathname</param>
        /// <param name="destinationFileName">destination pathname</param>
        /// <param name="allowOverwrite">set to true to overwrite an existing file</param>
        public void FileCopy(string sourceFileName, string destinationFileName, bool allowOverwrite)
        {
            if (!MtpPath.IsMtpPath(sourceFileName) && !MtpPath.IsMtpPath(destinationFileName))
            {
                _fileUtilities.FileCopy(sourceFileName, destinationFileName, allowOverwrite);
            }
            else
            {
                var sourceFileInfo = _fileInfoProvider.GetFileInfo(sourceFileName);

                using (var sourceStream = OpenReadStream(sourceFileName))
                {
                    using (var destinationStream = OpenWriteStream(destinationFileName, sourceFileInfo.Length, allowOverwrite))
                    {
                        _streamHelper.Copy(sourceStream, destinationStream);
                    }
                }
                
            }
        }

        /// <summary>
        /// delete a file
        /// </summary>
        /// <param name="path">pathname of the file to delete</param>
        public void FileDelete(string path)
        {
            var pathInfo = MtpPath.GetPathInfo(path);

            if (!pathInfo.IsMtpPath)
            {
                _fileUtilities.FileDelete(path);
                return;
            }

            var device = GetDevice(pathInfo);

            device.Delete(pathInfo.RelativePathOnDevice);
        }

        private IDevice GetDevice(MtpPathInfo pathInfo)
        {
            var device = _deviceManager.GetDevice(pathInfo.DeviceName);
            if (device == null)
            {
                throw new DirectoryNotFoundException(String.Format("Device [{0}] not found", pathInfo.DeviceName));
            }
            return device;
        }

        private Stream OpenReadStream(string filename)
        {
            var pathInfo = MtpPath.GetPathInfo(filename);

            if (pathInfo.IsMtpPath)
            {
                return GetDevice(pathInfo).OpenRead(pathInfo.RelativePathOnDevice);
            }

            return _streamHelper.OpenRead(filename);
        }

        private Stream OpenWriteStream(string filename, long length, bool allowOverwrite)
        {
            var pathInfo = MtpPath.GetPathInfo(filename);

            if (pathInfo.IsMtpPath)
            {
                return GetDevice(pathInfo).OpenWrite(pathInfo.RelativePathOnDevice, length, allowOverwrite);
            }

            return _streamHelper.OpenWrite(filename, allowOverwrite);
        }
    }
}