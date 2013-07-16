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
using PodcastUtilities.PortableDevices;

namespace PodcastUtilities.Common.Platform.Mtp
{
    ///<summary>
    /// Implementation of abstract file info for MTP "files" (child objects)
    ///</summary>
    public class FileInfo
        : IFileInfo
    {
        private readonly IDevice _device;
        private IDeviceObject _deviceObject;
        private readonly string _path;
        private DateTime _creationTime;

        internal FileInfo(IDevice device, string path)
            : this(device, null, path)
        {
        }

        internal FileInfo(IDevice device, IDeviceObject deviceObject, string path)
        {
            _device = device;
            _deviceObject = deviceObject;
            _path = path;
            _creationTime = DateTime.MinValue;
        }

        /// <summary>
        /// the name of the file eg. file.ext
        /// </summary>
        public string Name
        {
            get { return _deviceObject.Name; }
        }

        /// <summary>
        /// the full pathname of the object eg. c:\media\file.ext
        /// </summary>
        public string FullName
        {
            get { return MtpPath.MakeFullPath(MtpPath.Combine(_device.Name, _path)); }
        }

        /// <summary>
        /// date time the file was created
        /// </summary>
        public DateTime CreationTime
        {
            get
            {
                // TODO
                return _creationTime;
            }
        }

        ///<summary>
        /// Length of the file in bytes
        ///</summary>
        public long Length
        {
            get
            {
                // TODO
                return 0;
            }
        }

        private IDeviceObject DeviceObject
        {
            get
            {
                return _deviceObject ?? (_deviceObject = _device.GetObjectFromPath(_path));
            }
        }
    }
}