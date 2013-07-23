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
using System.IO;
using PortableDeviceApiLib;

namespace PodcastUtilities.PortableDevices
{
    /// <summary>
    /// represents an individual attached MTP device
    /// </summary>
    public class Device : IDevice
    {
        private readonly IPortableDeviceManager _portableDeviceManager;
        private readonly IPortableDeviceFactory _portableDeviceFactory;
        private readonly IPortableDeviceHelper _portableDeviceHelper;
        private readonly IDeviceStreamFactory _deviceStreamFactory;
        private IPortableDevice _portableDevice;
        private IPortableDeviceContent _portableDeviceContent;
        private string _name;

        internal Device(
            IPortableDeviceManager portableDeviceManager,
            IPortableDeviceFactory portableDeviceFactory, 
            IPortableDeviceHelper portableDeviceHelper,
            IDeviceStreamFactory deviceStreamFactory,
            string id)
        {
            _portableDeviceManager = portableDeviceManager;
            _portableDeviceFactory = portableDeviceFactory;
            _portableDeviceHelper = portableDeviceHelper;
            _deviceStreamFactory = deviceStreamFactory;
            Id = id;
            OpenDevice(id);
        }

        /// <summary>
        /// device ID set by the manufacturer
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// name of the device
        /// </summary>
        public string Name
        {
            get { return _name ?? (_name = GetDeviceName()); }
        }

        /// <summary>
        /// MTP device have named root objects
        /// </summary>
        /// <returns>all the root objects that support storage</returns>
        public IEnumerable<IDeviceObject> GetDeviceRootStorageObjects()
        {
            IList<IDeviceObject> returnValue = new List<IDeviceObject>(10);
            var parentId = PortableDeviceConstants.WPD_DEVICE_OBJECT_ID;

            var childObjectIds = _portableDeviceHelper.GetChildObjectIds(_portableDeviceContent, parentId);

            foreach (var id in childObjectIds)
            {
                if (IsStorageObject(id))
                {
                    var childObjectName = _portableDeviceHelper.GetObjectFileName(_portableDeviceContent, id);
                    returnValue.Add(new DeviceObject(_portableDeviceHelper, _portableDeviceContent, id, childObjectName));
                }
            }

            return returnValue;
        }

        /// <summary>
        /// get the object that coresponds to the path or NULL if it doesnt exist
        /// </summary>
        /// <param name="path">the path</param>
        /// <returns>the object or NULL</returns>
        public IDeviceObject GetObjectFromPath(string path)
        {
            var pathParts = path.Split(Path.DirectorySeparatorChar);

            var parentId = PortableDeviceConstants.WPD_DEVICE_OBJECT_ID;
            string childObjectId = null;
            string childObjectName = null;

            foreach (var part in pathParts)
            {
                childObjectId = GetChildObjectIdByName(parentId, part, out childObjectName);
                if (childObjectId == null)
                {
                    return null;
                }

                parentId = childObjectId;
            }

            return new DeviceObject(_portableDeviceHelper, _portableDeviceContent, childObjectId, childObjectName);
        }

        /// <summary>
        /// get the storage object that contains the leaf element of the path
        /// </summary>
        /// <param name="path">path to storage object</param>
        /// <returns></returns>
        public IDeviceObject GetRootStorageObjectFromPath(string path)
        {
            var pathParts = path.Split(Path.DirectorySeparatorChar);

            var parentId = PortableDeviceConstants.WPD_DEVICE_OBJECT_ID;
            string childObjectId = null;
            string childObjectName = null;

            foreach (var part in pathParts)
            {
                childObjectId = GetChildObjectIdByName(parentId, part, out childObjectName);
                if (childObjectId == null)
                {
                    return null;
                }

                if (IsStorageObject(childObjectId))
                {
                    return new DeviceObject(_portableDeviceHelper, _portableDeviceContent, childObjectId, childObjectName);
                }

                parentId = childObjectId;
            }

            return null;
        }

        /// <summary>
        /// creates all the required folders in the specified path
        /// </summary>
        /// <param name="path"></param>
        public void CreateFolderObjectFromPath(string path)
        {
            CreateFolderObject(path);
        }

        /// <summary>
        /// delete the object
        /// </summary>
        /// <param name="path"></param>
        public void Delete(string path)
        {
            var deviceObject = GetObjectFromPath(path);
            if (deviceObject == null)
            {
                // As File.Delete(), do not throw an exception if the file doesn't exist
                return;
            }

            _portableDeviceHelper.DeleteObject(_portableDeviceContent, deviceObject.Id);
        }

        /// <summary>
        /// open an object for reading
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public Stream OpenRead(string path)
        {
            var deviceObject = GetObjectFromPath(path);
            if (deviceObject == null)
            {
                throw new FileNotFoundException(String.Format("Device: [{0}]; path [{1}] not found", Name, path));
            }

            var resourceStream = _portableDeviceHelper.OpenResourceStream(
                _portableDeviceContent, 
                deviceObject.Id, 
                StreamConstants.STGM_READ);

            return _deviceStreamFactory.CreateStream(resourceStream);
        }

        /// <summary>
        /// open an object for writing
        /// </summary>
        /// <param name="path">path to object</param>
        /// <param name="length">number of bytes to store</param>
        /// <param name="allowOverwrite">true to overwrite</param>
        /// <returns></returns>
        public Stream OpenWrite(string path, long length, bool allowOverwrite)
        {
            var deviceObject = GetObjectFromPath(path);
            if (deviceObject != null)
            {
                if (!allowOverwrite)
                {
                    throw new IOException(String.Format("Device: [{0}]; path [{1}] already exists and allowOverwrite is false", Name, path));
                }

                _portableDeviceHelper.DeleteObject(_portableDeviceContent, deviceObject.Id);
            }

            var separatorPosition = path.LastIndexOf(Path.DirectorySeparatorChar);
            var fileName = path.Substring(separatorPosition + 1);

            var folderObjectId = PortableDeviceConstants.WPD_DEVICE_OBJECT_ID;
            if (separatorPosition >= 0)
            {
                var parentFolderPath = path.Substring(0, separatorPosition);
                folderObjectId = CreateFolderObject(parentFolderPath);
            }

            var resourceStream = _portableDeviceHelper.CreateResourceStream(
                _portableDeviceContent, 
                folderObjectId, 
                fileName,
                length);

            return _deviceStreamFactory.CreateStream(resourceStream);
        }

        private void OpenDevice(string id)
        {
            _portableDevice = _portableDeviceFactory.Create(id);
            _portableDevice.Content(out _portableDeviceContent);
        }

        private string GetDeviceName()
        {
            // _portableDeviceManager.GetDeviceFriendlyName() doesn't work for some devices (eg. my HTC One S)
            var name = _portableDeviceHelper.GetDeviceFriendlyName(_portableDeviceManager, Id);
            if (name != null)
            {
                return name;
            }
            
            return _portableDeviceHelper.GetObjectFileName(
                _portableDeviceContent,
                PortableDeviceConstants.WPD_DEVICE_OBJECT_ID);
        }

        private string GetChildObjectIdByName(string parentObjectId, string name, out string actualObjectName)
        {
            var childObjectIds = _portableDeviceHelper.GetChildObjectIds(_portableDeviceContent, parentObjectId);

            foreach (var id in childObjectIds)
            {
                var childObjectName = _portableDeviceHelper.GetObjectFileName(
                    _portableDeviceContent, 
                    id);
                if (string.Compare(childObjectName, name, true) == 0)
                {
                    actualObjectName = childObjectName;
                    return id;
                }
            }

            actualObjectName = null;
            return null;
        }

        private string CreateFolderObject(string path)
        {
            var pathParts = path.Split(Path.DirectorySeparatorChar);

            var parentId = PortableDeviceConstants.WPD_DEVICE_OBJECT_ID;
            string childObjectId = null;
            string childObjectName = null;
            foreach (var part in pathParts)
            {
                childObjectId = GetChildObjectIdByName(parentId, part, out childObjectName);
                if (childObjectId == null)
                {
                    // we have reached the end of folders that exist - so we need to start creating from here
                    childObjectId = _portableDeviceHelper.CreateFolderObject(_portableDeviceContent, parentId, part);
                }

                parentId = childObjectId;
            }

            return parentId;
        }

        private bool IsStorageObject(string objectId)
        {
            var contentType = _portableDeviceHelper.GetObjectContentType(_portableDeviceContent, objectId);
            if (contentType == PortableDeviceConstants.WPD_CONTENT_TYPE_FUNCTIONAL_OBJECT)
            {
                var functionalCategory = _portableDeviceHelper.GetGuidProperty(
                    _portableDeviceContent, 
                    objectId,
                    PortableDevicePropertyKeys.WPD_FUNCTIONAL_OBJECT_CATEGORY);
                if (functionalCategory == PortableDeviceConstants.WPD_FUNCTIONAL_CATEGORY_STORAGE)
                {
                    return true;
                }
            }

            return false;
        }
    }
}