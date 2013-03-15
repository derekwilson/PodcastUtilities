using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PortableDeviceApiLib;

namespace MtpSpike
{
    class Program
    {
        static void Main(string[] args)
        {
            var deviceManager = new PortableDeviceManagerClass();

            var devices = EnumerateAllDevices(deviceManager);

            foreach (var device in devices)
            {
                Console.WriteLine("========================================");
                Console.WriteLine("ID:\t{0}", device.Id);
                Console.WriteLine("Name:\t{0}", device.FriendlyName);
                Console.WriteLine("Description:\t{0}", device.Description);
                Console.WriteLine();

                EnumerateObjects(deviceManager, device.Id);

                Console.WriteLine("========================================");
                Console.WriteLine();
            }
        }

        private static List<PortableDevice> EnumerateAllDevices(IPortableDeviceManager deviceManager)
        {
            uint deviceCount = 0;

            deviceManager.RefreshDeviceList();
            deviceManager.GetDevices(null, ref deviceCount);

            var devices = new List<PortableDevice>();

            if (deviceCount == 0)
            {
                return devices;
            }

            var deviceIds = new string[deviceCount];

            deviceManager.GetDevices(deviceIds, ref deviceCount);

            devices.AddRange(deviceIds.Select(id => CreateDevice(deviceManager, id)));

            return devices;
        }

        private static PortableDevice CreateDevice(IPortableDeviceManager deviceManager, string deviceId)
        {
            return new PortableDevice
                       {
                           Id = deviceId,
                           FriendlyName = GetFriendlyName(deviceManager, deviceId),
                           Description = GetDescription(deviceManager, deviceId)
                       };
        }

        private static void EnumerateObjects(PortableDeviceManagerClass deviceManager, string deviceId)
        {
            var device = OpenDevice(deviceId);

            IPortableDeviceContent deviceContent;
            device.Content(out deviceContent);

            EnumerateContent(PortableDeviceConstants.WPD_DEVICE_OBJECT_ID, deviceContent, "");
        }

        private static IPortableDevice OpenDevice(string deviceId)
        {
            var deviceValues = (IPortableDeviceValues)new PortableDeviceTypesLib.PortableDeviceValuesClass();

            deviceValues.SetStringValue(ref PortableDevicePropertyKeys.WPD_CLIENT_NAME, "Test MTP Client");
            deviceValues.SetUnsignedIntegerValue(ref PortableDevicePropertyKeys.WPD_CLIENT_MAJOR_VERSION, 1);
            deviceValues.SetUnsignedIntegerValue(ref PortableDevicePropertyKeys.WPD_CLIENT_MINOR_VERSION, 0);
            deviceValues.SetUnsignedIntegerValue(ref PortableDevicePropertyKeys.WPD_CLIENT_REVISION, 1);

            var device = new PortableDeviceClass();

            device.Open(deviceId, deviceValues);

            return device;
        }

        private static void EnumerateContent(string objectId, IPortableDeviceContent deviceContent, string indent)
        {
            const int numberOfObjects = 1;

            //var name = GetObjectName(deviceContent, objectId);
            //Console.WriteLine("{0}{1} : {2}", indent, objectId, name);

            Console.WriteLine("{0}{1}", indent, objectId);

            var properties = GetObjectProperties(deviceContent, objectId);
            foreach (var property in properties)
            {
                Console.WriteLine("{0}- {1}", indent, property);
            }

            indent += "  ";

            IEnumPortableDeviceObjectIDs objectIdEnumerator;
            deviceContent.EnumObjects(0, objectId, null, out objectIdEnumerator);

            uint numberReturned;

            do
            {
                numberReturned = 0;
                string childObjectId;
                objectIdEnumerator.Next(numberOfObjects, out childObjectId, ref numberReturned);

                if (numberReturned != 0)
                {
                    EnumerateContent(childObjectId, deviceContent, indent);
                }

            } while (numberReturned != 0);
        }

//        private static string GetObjectName(IPortableDeviceContent deviceContent, string objectId)
//        {
//            IPortableDeviceProperties deviceProperties;
//            deviceContent.Properties(out deviceProperties);
//
//            var keyCollection = (IPortableDeviceKeyCollection)new PortableDeviceTypesLib.PortableDeviceKeyCollectionClass();
//            keyCollection.Add(ref PortableDevicePropertyKeys.WPD_OBJECT_NAME);
//
//            IPortableDeviceValues deviceValues;
//            deviceProperties.GetValues(objectId, keyCollection, out deviceValues);
//
//            string name;
//            deviceValues.GetStringValue(ref PortableDevicePropertyKeys.WPD_OBJECT_NAME, out name);
//
//            return name;
//        }

        private static IEnumerable<string> GetObjectProperties(IPortableDeviceContent deviceContent, string objectId)
        {
            IPortableDeviceProperties deviceProperties;
            deviceContent.Properties(out deviceProperties);

            IPortableDeviceValues deviceValues;
            deviceProperties.GetValues(objectId, null, out deviceValues);

            var properties = new List<string>();

            uint valueCount = 0;
            deviceValues.GetCount(ref valueCount);

            for (uint i = 0; i < valueCount; i++ )
            {
                var key = new _tagpropertykey();
                var value = new tag_inner_PROPVARIANT();
                deviceValues.GetAt(i, ref key, ref value);

                properties.Add(
                    String.Format("[{0}, {1}] : {2}", key.fmtid, key.pid, PropVariant.FromValue(value).AsString()));
            }


            return properties;
        }

        private static string GetFriendlyName(
            IPortableDeviceManager deviceManager,
            string deviceId)
        {
            return GetDeviceStringProperty(
                deviceManager,
                deviceId,
                (dm, id, value, count) =>
                {
                    dm.GetDeviceFriendlyName(id, value, ref count);
                    return count;
                });
        }

        private static string GetDescription(
            IPortableDeviceManager deviceManager,
            string deviceId)
        {
            return GetDeviceStringProperty(
                deviceManager,
                deviceId,
                (dm, id, value, count) =>
                    {
                        dm.GetDeviceDescription(id, value, ref count);
                        return count;
                    });
        }

        private static string GetDeviceStringProperty(
            IPortableDeviceManager deviceManager,
            string deviceId,
            Func<IPortableDeviceManager, string, ushort[], uint, uint> propertyGetter)
        {
            uint propertyValueCount = 0;

            try
            {
                // First, pass NULL to get the total number of characters to allocate for the string value.
                propertyValueCount = propertyGetter(deviceManager, deviceId, null, 0);
            }
            catch (Exception exception)
            {
                Console.WriteLine("-- Exception reading property for device {0}", deviceId);
                Console.WriteLine(exception);
                Console.WriteLine();
            }

            if (propertyValueCount == 0)
            {
                return "";
            }

            var propertyValue = new ushort[propertyValueCount];

            propertyGetter(deviceManager, deviceId, propertyValue, propertyValueCount);

            return ConvertToString(propertyValue);
        }

        private static string ConvertToString(ushort[] characters)
        {
            var builder = new StringBuilder();

            builder.Append(characters.Select(c => (char) c).ToArray());

            return builder.ToString();
        }
    }
}
