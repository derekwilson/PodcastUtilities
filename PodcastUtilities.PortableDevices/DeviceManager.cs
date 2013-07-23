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
using System.Linq;
using PortableDeviceApiLib;

namespace PodcastUtilities.PortableDevices
{
    /// <summary>
    /// represents the mechanism for manipulating all MTP devices currently attached
    /// </summary>
    public class DeviceManager : IDeviceManager
    {
        private readonly IPortableDeviceManager _portableDeviceManager;
        private readonly IDeviceFactory _deviceFactory;

        private Dictionary<string, IDevice> _deviceNameCache;
        private Dictionary<string, IDevice> _deviceIdCache;

        /// <summary>
        /// create a device manager
        /// </summary>
        public DeviceManager()
            : this(new PortableDeviceManagerClass())
        {
        }

        internal DeviceManager(
            IPortableDeviceManager portableDeviceManager)
            : this(portableDeviceManager, new DeviceFactory(portableDeviceManager))
        {
        }

        internal DeviceManager(
            IPortableDeviceManager portableDeviceManager,
            IDeviceFactory deviceFactory)
        {
            _portableDeviceManager = portableDeviceManager;
            _deviceFactory = deviceFactory;
        }

        /// <summary>
        /// gets a specific device
        /// </summary>
        /// <param name="deviceName">the name of the device</param>
        /// <returns>the device</returns>
        public IDevice GetDevice(string deviceName)
        {
            EnumerateDevices();

            IDevice device;
            _deviceNameCache.TryGetValue(deviceName, out device);

            return device;
        }

        /// <summary>
        /// Gets all the attached devices
        /// </summary>
        /// <returns>all the currently attached devices</returns>
        public IEnumerable<IDevice> GetAllDevices()
        {
            EnumerateDevices();
            return _deviceNameCache.Values.AsEnumerable();
        }

        private void EnumerateDevices()
        {
            if ((_deviceNameCache != null) && (_deviceIdCache != null))
            {
                return;
            }

            uint deviceCount = 0;
            _portableDeviceManager.GetDevices(null, ref deviceCount);

            var deviceIds = new string[deviceCount];
            _portableDeviceManager.GetDevices(deviceIds, ref deviceCount);

            var devices = deviceIds.Select(CreateDevice).ToList();

            _deviceNameCache = devices.ToDictionary(device => device.Name);
            _deviceIdCache = devices.ToDictionary(device => device.Id);
        }

        private IDevice CreateDevice(string id)
        {
            return _deviceFactory.CreateDevice(id);
        }
    }
}