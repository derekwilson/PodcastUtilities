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
using System.Collections.Generic;
using System.Linq;
using PortableDeviceApiLib;

namespace PodcastUtilities.PortableDevices
{
    /// <summary>
    /// an object on a device
    /// </summary>
    internal class DeviceObject : IDeviceObject
    {
        private readonly IPortableDeviceHelper _portableDeviceHelper;
        private readonly IPortableDeviceContent _portableDeviceContent;
        private readonly IFilenameMatcher _filenameMatcher;

        public DeviceObject(
            IPortableDeviceHelper portableDeviceHelper, 
            IPortableDeviceContent portableDeviceContent, 
            string id, 
            string name)
            : this(portableDeviceHelper, portableDeviceContent, new FilenameMatcher(), id, name)
        {
        }

        public DeviceObject(
            IPortableDeviceHelper portableDeviceHelper, 
            IPortableDeviceContent portableDeviceContent, 
            IFilenameMatcher filenameMatcher,
            string id, 
            string name)
        {
            _portableDeviceHelper = portableDeviceHelper;
            _portableDeviceContent = portableDeviceContent;
            _filenameMatcher = filenameMatcher;
            Id = id;
            Name = name;
        }

        /// <summary>
        /// unique id
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// readable name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// free space
        /// Only relevant for storage objects - so maybe shouldn't be here...
        /// </summary>
        public long AvailableFreeSpace
        {
            get
            {
                return (long) _portableDeviceHelper.GetUnsignedLargeIntegerProperty(
                    _portableDeviceContent,
                    Id,
                    PortableDevicePropertyKeys.WPD_STORAGE_FREE_SPACE_IN_BYTES);
            }
        }

        /// <summary>
        /// gets all the folder objects
        /// </summary>
        /// <param name="pattern">pattern to match</param>
        /// <returns>folder objects</returns>
        public IEnumerable<IDeviceObject> GetFolders(string pattern)
        {
            // TODO: caching

            var childObjectIds = GetFilteredChildObjectIds(id => IsFolder(id) && IsObjectFilenameMatch(id, pattern));

            return childObjectIds.Select(CreateDeviceObject);
        }

        /// <summary>
        /// gets all the file objects
        /// </summary>
        /// <param name="pattern">pattern to match</param>
        /// <returns>file objects</returns>
        public IEnumerable<IDeviceObject> GetFiles(string pattern)
        {
            // TODO: caching

            var childObjectIds = GetFilteredChildObjectIds(id => IsFile(id) && IsObjectFilenameMatch(id, pattern));

            return childObjectIds.Select(CreateDeviceObject);
        }

        private IEnumerable<string> GetFilteredChildObjectIds(Predicate<string> match)
        {
            return _portableDeviceHelper.GetChildObjectIds(_portableDeviceContent, Id).Where(id => match(id));
        }

        private bool IsFolder(string objectId)
        {
            var contentType = _portableDeviceHelper.GetObjectContentType(_portableDeviceContent, objectId);
            return ((contentType == PortableDeviceConstants.WPD_CONTENT_TYPE_FOLDER) ||
                    (contentType == PortableDeviceConstants.WPD_CONTENT_TYPE_FUNCTIONAL_OBJECT));
        }

        private bool IsFile(string objectId)
        {
            return !IsFolder(objectId);
        }

        private bool IsObjectFilenameMatch(string id, string pattern)
        {
            var filename = _portableDeviceHelper.GetObjectFileName(_portableDeviceContent, id);

            return _filenameMatcher.IsMatch(filename, pattern);
        }

        private IDeviceObject CreateDeviceObject(string objectId)
        {
            var objectName = _portableDeviceHelper.GetObjectFileName(_portableDeviceContent, objectId);

            return new DeviceObject(_portableDeviceHelper, _portableDeviceContent, objectId, objectName);
        }
    }
}