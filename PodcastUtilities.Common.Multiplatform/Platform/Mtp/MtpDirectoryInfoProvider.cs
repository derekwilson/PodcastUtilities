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
    /// <summary>
    /// MTP implementation of directory info provider
    /// </summary>
    public class MtpDirectoryInfoProvider
        : IDirectoryInfoProvider
    {
        private readonly IDeviceManager _deviceManager;

        ///<summary>
        /// Constructs the directory info provider
        ///</summary>
        ///<param name="deviceManager"></param>
        public MtpDirectoryInfoProvider(IDeviceManager deviceManager)
        {
            _deviceManager = deviceManager;
        }

        /// <summary>
        /// create an abstract directory object
        /// </summary>
        /// <param name="path">full path to the directory</param>
        /// <returns>an abstrcat object</returns>
        public IDirectoryInfo GetDirectoryInfo(string path)
        {
            var pathInfo = MtpPath.GetPathInfo(path);

            var device = _deviceManager.GetDevice(pathInfo.DeviceName);

            if (device == null)
            {
                throw new DirectoryNotFoundException(String.Format("Device [{0}] not found", pathInfo.DeviceName));
            }

            return new DirectoryInfo(device, pathInfo.RelativePathOnDevice);
        }
    }
}