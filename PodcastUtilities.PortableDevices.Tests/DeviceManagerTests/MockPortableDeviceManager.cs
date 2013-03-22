using System;
using PortableDeviceApiLib;

namespace PodcastUtilities.PortableDevices.Tests.DeviceManagerTests
{
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