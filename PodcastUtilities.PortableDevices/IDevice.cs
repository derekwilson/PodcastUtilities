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
using System.Collections.Generic;
using System.IO;

namespace PodcastUtilities.PortableDevices
{
    /// <summary>
    /// represents an individual attached MTP device
    /// </summary>
    public interface IDevice
    {
        /// <summary>
        /// device ID set by the manufacturer
        /// </summary>
        string Id { get; }
        /// <summary>
        /// name of the device
        /// </summary>
        string Name { get; }

        /// <summary>
        /// MTP device have named root objects
        /// </summary>
        /// <returns>all the root objects that support storage</returns>
        IEnumerable<IDeviceObject> GetDeviceRootStorageObjects();

        /// <summary>
        /// get the object that coresponds to the path or NULL if it doesnt exist
        /// </summary>
        /// <param name="path">the path</param>
        /// <returns>the object or NULL</returns>
        IDeviceObject GetObjectFromPath(string path);
        
        /// <summary>
        /// get the storage object that contains the leaf element of the path
        /// </summary>
        /// <param name="path">path to storage object</param>
        /// <returns></returns>
        IDeviceObject GetRootStorageObjectFromPath(string path);
        
        /// <summary>
        /// delete the object
        /// </summary>
        /// <param name="path"></param>
        void Delete(string path);
        
        /// <summary>
        /// open an object for reading
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        Stream OpenRead(string path);
        
        /// <summary>
        /// open an object for writing
        /// </summary>
        /// <param name="path">path to object</param>
        /// <param name="length">number of bytes to store</param>
        /// <param name="allowOverwrite">true to overwrite</param>
        /// <returns></returns>
        Stream OpenWrite(string path, long length, bool allowOverwrite);
        
        /// <summary>
        /// creates all the rewuired folders in the specified path
        /// </summary>
        /// <param name="path"></param>
        void CreateFolderObjectFromPath(string path);
    }
}