using PortableDeviceApiLib;

namespace PodcastUtilities.PortableDevices
{
    internal class PortableDeviceFactory : IPortableDeviceFactory
    {
        public IPortableDevice Create(string deviceId)
        {
            var deviceValues = (IPortableDeviceValues)new PortableDeviceTypesLib.PortableDeviceValuesClass();

            deviceValues.SetStringValue(ref PortableDevicePropertyKeys.WPD_CLIENT_NAME, "PodcastUtilities.PortableDevices");
            deviceValues.SetUnsignedIntegerValue(ref PortableDevicePropertyKeys.WPD_CLIENT_MAJOR_VERSION, 1);
            deviceValues.SetUnsignedIntegerValue(ref PortableDevicePropertyKeys.WPD_CLIENT_MINOR_VERSION, 0);
            deviceValues.SetUnsignedIntegerValue(ref PortableDevicePropertyKeys.WPD_CLIENT_REVISION, 1);

            var device = new PortableDeviceClass();

            device.Open(deviceId, deviceValues);

            return device;
        }
    }
}