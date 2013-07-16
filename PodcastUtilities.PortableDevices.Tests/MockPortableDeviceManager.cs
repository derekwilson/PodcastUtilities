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
using PortableDeviceApiLib;

namespace PodcastUtilities.PortableDevices.Tests
{
    // Using a generated stub doesn't work as the stub seems to get confused with
    // all the Marshalling gubbins on the interface.
    public class MockPortableDeviceManager : IPortableDeviceManager
    {
        public void GetDevices(string[] pPnPDeviceIDs, ref uint pcPnPDeviceIDs)
        {
            pcPnPDeviceIDs = 2;

            if (pPnPDeviceIDs == null)
            {
                return;
            }

            var mockDeviceIds = new[] { "Device_Id_1", "Device_Id_2" };

            mockDeviceIds.CopyTo(pPnPDeviceIDs, 0);
        }

        public void RefreshDeviceList()
        {
            throw new NotImplementedException();
        }

        public void GetDeviceFriendlyName(string pszPnPDeviceID, ushort[] pDeviceFriendlyName, ref uint pcchDeviceFriendlyName)
        {
            throw new NotImplementedException();
        }

        public void GetDeviceDescription(string pszPnPDeviceID, ushort[] pDeviceDescription, ref uint pcchDeviceDescription)
        {
            throw new NotImplementedException();
        }

        public void GetDeviceManufacturer(string pszPnPDeviceID, ushort[] pDeviceManufacturer, ref uint pcchDeviceManufacturer)
        {
            throw new NotImplementedException();
        }

        public void GetDeviceProperty(string pszPnPDeviceID, string pszDevicePropertyName, ref byte pData, ref uint pcbData, ref uint pdwType)
        {
            throw new NotImplementedException();
        }

        public void GetPrivateDevices(ref string pPnPDeviceIDs, ref uint pcPnPDeviceIDs)
        {
            throw new NotImplementedException();
        }
    }
}