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
using System.Linq;
using PodcastUtilities.PortableDevices;

namespace PodcastUtilities.Common.Platform.Mtp
{
    ///<summary>
    /// Implementation of abstract directory info for MTP "directories" (parent objects)
    ///</summary>
    public class DirectoryInfo
        : IDirectoryInfo
    {
        private readonly IDevice _device;
        private IDeviceObject _deviceObject;
        private readonly string _path;

        internal DirectoryInfo(IDevice device, string pathToObject)
            : this(device, null, pathToObject)
        {
        }

        internal DirectoryInfo(IDevice device, IDeviceObject deviceObject, string pathToObject)
        {
            _device = device;
            _deviceObject = deviceObject;
            _path = pathToObject;
        }

        /// <summary>
        /// the full pathname of the directory
        /// </summary>
        public string FullName
        {
            get { return MtpPath.MakeFullPath(MtpPath.Combine(_device.Name, _path)); }
        }

        /// <summary>
        /// true if it exists
        /// </summary>
        public bool Exists
        {
            get { return DeviceObject != null; }
        }

        private IDeviceObject DeviceObject
        {
            get
            {
                return _deviceObject ?? (_deviceObject = _device.GetObjectFromPath(_path));
            }
        }

        /// <summary>
        /// gets an abstract collection of files that are contained by by the directory
        /// </summary>
        /// <param name="pattern">a search patter for example *.mp3</param>
        /// <returns>a collection of abstracted files</returns>
        public IFileInfo[] GetFiles(string pattern)
        {
            CheckObjectExists();

            var childFileObjects = DeviceObject.GetFiles(pattern);

            return childFileObjects.Select(file => new FileInfo(_device, file, MtpPath.Combine(_path, file.Name))).ToArray();
        }

        /// <summary>
        /// gets an abstract collection of directories that are contained by the directory
        /// </summary>
        /// <param name="pattern">a search patter for example *.*</param>
        /// <returns>a collection of abstracted files</returns>
        public IDirectoryInfo[] GetDirectories(string pattern)
        {
            CheckObjectExists();

            var childFolderObjects = DeviceObject.GetFolders(pattern);

            return childFolderObjects.Select(folder => new DirectoryInfo(_device, folder, MtpPath.Combine(_path, folder.Name))).ToArray();
        }

        /// <summary>
        /// create the directory in the file system
        /// </summary>
        public void Create()
        {
            if (Exists)
            {
                // nothing to do
                return;
            }
            _device.CreateFolderObjectFromPath(_path);
        }

        /// <summary>
        /// delete the directory in the file system
        /// </summary>
        public void Delete()
        {
            if (Exists)
            {
                _device.Delete(_path);
                _deviceObject = null;   // force it to be refreshed
            }
        }

        private void CheckObjectExists()
        {
            if (!Exists)
            {
                throw new DirectoryNotFoundException(String.Format("Path [{0}] not found on device [{1}]", _path, _device.Name));
            }
        }
    }
}