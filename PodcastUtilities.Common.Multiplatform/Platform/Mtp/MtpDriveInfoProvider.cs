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
using PodcastUtilities.PortableDevices;

namespace PodcastUtilities.Common.Platform.Mtp
{
    ///<summary>
    /// MTP implementation of drive info provider
    ///</summary>
    public class MtpDriveInfoProvider
        : IDriveInfoProvider
    {
        private readonly IDeviceManager _deviceManager;

        ///<summary>
        /// Constructs the drive info provider
        ///</summary>
        ///<param name="deviceManager"></param>
        public MtpDriveInfoProvider(IDeviceManager deviceManager)
        {
            _deviceManager = deviceManager;
        }

        /// <summary>
        /// create an abstract drive info object
        /// </summary>
        /// <param name="path">name of the drive</param>
        /// <returns>an abstrcat object</returns>
        public IDriveInfo GetDriveInfoForPath(string path)
        {
            var pathInfo = MtpPath.GetPathInfo(path);

            var device = _deviceManager.GetDevice(pathInfo.DeviceName);

            if (device == null)
            {
                throw new DriveNotFoundException(String.Format("Device [{0}] not found", pathInfo.DeviceName));
            }

            var storageObject = device.GetRootStorageObjectFromPath(pathInfo.RelativePathOnDevice);
            if (storageObject == null)
            {
                throw new DriveNotFoundException(
                    String.Format(
                        "No storage object found: Device [{0}], path [{1}]", 
                        pathInfo.DeviceName,
                        pathInfo.RelativePathOnDevice));
            }

            return new DriveInfo(device, storageObject);
        }
    }
}