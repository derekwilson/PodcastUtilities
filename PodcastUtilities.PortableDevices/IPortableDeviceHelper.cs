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
using PortableDeviceApiLib;

namespace PodcastUtilities.PortableDevices
{
    /// <summary>
    /// wrapper functions to call into unmanaged wrapper
    /// </summary>
    [CLSCompliant(false)]
    public interface IPortableDeviceHelper
    {
        /// <summary>
        /// turns a manufacturers id into a friendly name
        /// </summary>
        /// <param name="portableDeviceManager">device manager</param>
        /// <param name="deviceId">the id</param>
        /// <returns>name</returns>
        string GetDeviceFriendlyName(IPortableDeviceManager portableDeviceManager, string deviceId);

        /// <summary>
        /// Tries to get the original filename property if it exists (ie. for real files/directories), falls
        /// back to object name for non-file objects (eg. device, Internal Storage, etc.)
        /// </summary>
        /// <param name="deviceContent">unmanaged device</param>
        /// <param name="objectId">the object id</param>
        /// <returns>name</returns>
        string GetObjectFileName(IPortableDeviceContent deviceContent, string objectId);

        /// <summary>
        /// gets the name of an object
        /// </summary>
        /// <param name="deviceContent">unmanged device</param>
        /// <param name="objectId">object id</param>
        /// <returns>name</returns>
        string GetObjectName(IPortableDeviceContent deviceContent, string objectId);

        /// <summary>
        /// identify the object type
        /// </summary>
        /// <param name="deviceContent">unmanged device</param>
        /// <param name="objectId">the object id</param>
        /// <returns>content type guid</returns>
        Guid GetObjectContentType(IPortableDeviceContent deviceContent, string objectId);

        /// <summary>
        /// gets the create date time for the object
        /// </summary>
        /// <param name="deviceContent">unmanged device</param>
        /// <param name="objectId">object id</param>
        /// <returns>Try the creation date, fall back to modified date</returns>
        DateTime GetObjectCreationTime(IPortableDeviceContent deviceContent, string objectId);

        /// <summary>
        /// lookup the given key and return the string property associated with the object
        /// </summary>
        /// <param name="deviceContent">unmanged device</param>
        /// <param name="objectId">object id</param>
        /// <param name="key">name of the property key</param>
        /// <returns>string value</returns>
        string GetStringProperty(IPortableDeviceContent deviceContent, string objectId, _tagpropertykey key);

        /// <summary>
        /// lookup the given key and return the guid property associated with the object
        /// </summary>
        /// <param name="deviceContent">unmanged device</param>
        /// <param name="objectId">object id</param>
        /// <param name="key">name of the property key</param>
        /// <returns>guid value</returns>
        Guid GetGuidProperty(IPortableDeviceContent deviceContent, string objectId, _tagpropertykey key);

        /// <summary>
        /// lookup the given key and return the long property associated with the object
        /// </summary>
        /// <param name="deviceContent">unmanged device</param>
        /// <param name="objectId">object id</param>
        /// <param name="key">name of the property key</param>
        /// <returns>long value</returns>
        ulong GetUnsignedLargeIntegerProperty(IPortableDeviceContent deviceContent, string objectId, _tagpropertykey key);

        /// <summary>
        /// lookup the given key and return the date property associated with the object
        /// </summary>
        /// <param name="deviceContent">unmanged device</param>
        /// <param name="objectId">object id</param>
        /// <param name="key">name of the property key</param>
        /// <returns>date value</returns>
        DateTime GetDateProperty(IPortableDeviceContent deviceContent, string objectId, _tagpropertykey key);

        /// <summary>
        /// get all the child object ids from a given parent
        /// </summary>
        /// <param name="deviceContent">unmanged device</param>
        /// <param name="parentId">parent object id</param>
        /// <returns>all child ids</returns>
        IEnumerable<string> GetChildObjectIds(IPortableDeviceContent deviceContent, string parentId);

        /// <summary>
        /// create a folder storage object withing the specified parent
        /// </summary>
        /// <param name="deviceContent">unmanaged device</param>
        /// <param name="parentObjectId">parent object id</param>
        /// <param name="newFolder">name of the new folder</param>
        /// <returns>created object id</returns>
        string CreateFolderObject(IPortableDeviceContent deviceContent, string parentObjectId, string newFolder);

        /// <summary>
        /// delete the specified object
        /// </summary>
        /// <param name="deviceContent">unmanged device</param>
        /// <param name="objectId">object to delete</param>
        void DeleteObject(IPortableDeviceContent deviceContent, string objectId);

        /// <summary>
        /// open a stream on the device for reading
        /// </summary>
        /// <param name="deviceContent">unmanged device</param>
        /// <param name="objectId">object to open</param>
        /// <param name="mode">mode to open the stream in</param>
        /// <returns></returns>
        IStream OpenResourceStream(IPortableDeviceContent deviceContent, string objectId, uint mode);

        /// <summary>
        /// create a new resource stream in a parent object
        /// </summary>
        /// <param name="deviceContent">unmanged device</param>
        /// <param name="parentObjectId">the parent object</param>
        /// <param name="fileName">file to create</param>
        /// <param name="length">length of the file in bytes</param>
        /// <returns></returns>
        IStream CreateResourceStream(IPortableDeviceContent deviceContent, string parentObjectId, string fileName, long length);
    }
}