using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PortableDeviceApiLib;

namespace PodcastUtilities.PortableDevices
{
    internal class PortableDeviceHelper : IPortableDeviceHelper
    {
        public string GetDeviceFriendlyName(IPortableDeviceManager portableDeviceManager, string deviceId)
        {
            return GetDeviceStringProperty(
                portableDeviceManager,
                deviceId,
                (dm, id, value, count) =>
                {
                    dm.GetDeviceFriendlyName(id, value, ref count);
                    return count;
                });
        }

        public string GetObjectFileName(IPortableDeviceContent deviceContent, string objectId)
        {
            // Tries to get the original filename property if it exists (ie. for real files/directories), falls
            // back to object name for non-file objects (eg. device, Internal Storage, etc.)

            try
            {
                return GetStringProperty(
                    deviceContent,
                    objectId,
                    PortableDevicePropertyKeys.WPD_OBJECT_ORIGINAL_FILE_NAME);
            }
            catch (Exception)
            {
                return GetObjectName(deviceContent, objectId);
            }
        }

        public string GetObjectName(IPortableDeviceContent deviceContent, string objectId)
        {
            return GetStringProperty(
                deviceContent, 
                objectId,
                PortableDevicePropertyKeys.WPD_OBJECT_NAME);
        }

        public Guid GetObjectContentType(IPortableDeviceContent deviceContent, string objectId)
        {
            return GetGuidProperty(
                deviceContent, 
                objectId,
                PortableDevicePropertyKeys.WPD_OBJECT_CONTENT_TYPE);
        }

        public DateTime GetObjectCreationTime(IPortableDeviceContent deviceContent, string objectId)
        {
            // Try the creation date, fall back to modified date
            try
            {
                return GetDateProperty(
                    deviceContent,
                    objectId,
                    PortableDevicePropertyKeys.WPD_OBJECT_DATE_CREATED);
            }
            catch (Exception)
            {
                return GetDateProperty(
                    deviceContent,
                    objectId,
                    PortableDevicePropertyKeys.WPD_OBJECT_DATE_MODIFIED);
            }
        }

        public string GetStringProperty(IPortableDeviceContent deviceContent, string objectId, _tagpropertykey key)
        {
            var deviceValues = GetDeviceValues(deviceContent, key, objectId);

            string value;
            deviceValues.GetStringValue(ref key, out value);

            return value;
        }

        public Guid GetGuidProperty(IPortableDeviceContent deviceContent, string objectId, _tagpropertykey key)
        {
            var deviceValues = GetDeviceValues(deviceContent, key, objectId);

            Guid value;
            deviceValues.GetGuidValue(ref key, out value);

            return value;
        }

        public DateTime GetDateProperty(IPortableDeviceContent deviceContent, string objectId, _tagpropertykey key)
        {
            var deviceValues = GetDeviceValues(deviceContent, key, objectId);

            tag_inner_PROPVARIANT value;
            deviceValues.GetValue(ref key, out value);

            return PropVariant.FromValue(value).ToDate();
        }

        public IEnumerable<string> GetChildObjectIds(IPortableDeviceContent deviceContent, string parentId)
        {
            var childObjectIds = new List<string>();

            IEnumPortableDeviceObjectIDs objectIdEnumerator;
            deviceContent.EnumObjects(0, parentId, null, out objectIdEnumerator);

            const int numberOfObjects = 1;
            uint numberReturned;

            do
            {
                numberReturned = 0;
                string childObjectId;
                objectIdEnumerator.Next(numberOfObjects, out childObjectId, ref numberReturned);

                if (numberReturned != 0)
                {
                    childObjectIds.Add(childObjectId);
                }

            } while (numberReturned != 0);

            return childObjectIds;
        }

        private void GetRequiredPropertiesForFolder(string parentObjectId, string folderName, ref  IPortableDeviceValues ppObjectProperties)
        {
            ppObjectProperties.SetStringValue(ref PortableDevicePropertyKeys.WPD_OBJECT_PARENT_ID, parentObjectId);
            ppObjectProperties.SetStringValue(ref PortableDevicePropertyKeys.WPD_OBJECT_NAME, folderName);
            ppObjectProperties.SetStringValue(ref PortableDevicePropertyKeys.WPD_OBJECT_ORIGINAL_FILE_NAME, folderName);

            ppObjectProperties.SetGuidValue(ref PortableDevicePropertyKeys.WPD_OBJECT_CONTENT_TYPE, ref PortableDeviceConstants.WPD_CONTENT_TYPE_FOLDER);
        }

        public string CreateFolderObject(IPortableDeviceContent deviceContent, string parentObjectId, string newFolder)
        {
            IPortableDeviceProperties deviceProperties;
            deviceContent.Properties(out deviceProperties);
            
            IPortableDeviceValues deviceValues = (IPortableDeviceValues)new PortableDeviceTypesLib.PortableDeviceValues();
            GetRequiredPropertiesForFolder(parentObjectId, newFolder, ref deviceValues);

            string objectId = string.Empty;
            deviceContent.CreateObjectWithPropertiesOnly(deviceValues,ref objectId);
            return objectId;
        }

        public void DeleteObject(IPortableDeviceContent deviceContent, string objectId)
        {
            var objectIdCollection =
                (IPortableDevicePropVariantCollection)new PortableDeviceTypesLib.PortableDevicePropVariantCollectionClass();

            var propVariantValue = PropVariant.StringToPropVariant(objectId);
            objectIdCollection.Add(ref propVariantValue);

            // TODO: get the results back and handle failures correctly
            deviceContent.Delete(PortableDeviceConstants.PORTABLE_DEVICE_DELETE_NO_RECURSION, objectIdCollection, null);
        }

        public IStream OpenResourceStream(IPortableDeviceContent deviceContent, string objectId, uint mode)
        {
            IPortableDeviceResources resources;
            deviceContent.Transfer(out resources);

            IStream stream;
            uint optimalBufferSize = 0;
            resources.GetStream(objectId, PortableDevicePropertyKeys.WPD_RESOURCE_DEFAULT, mode, ref optimalBufferSize, out stream);

            return stream;
        }

        private static IPortableDeviceValues GetDeviceValues(IPortableDeviceContent deviceContent, _tagpropertykey key, string objectId)
        {
            IPortableDeviceProperties deviceProperties;
            deviceContent.Properties(out deviceProperties);

            var keyCollection = (IPortableDeviceKeyCollection)new PortableDeviceTypesLib.PortableDeviceKeyCollectionClass();
            keyCollection.Add(key);

            IPortableDeviceValues deviceValues;
            deviceProperties.GetValues(objectId, keyCollection, out deviceValues);
            return deviceValues;
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
                return null;
            }

            var propertyValue = new ushort[propertyValueCount];

            propertyGetter(deviceManager, deviceId, propertyValue, propertyValueCount);

            return ConvertToString(propertyValue);
        }

        private static string ConvertToString(ushort[] characters)
        {
            var builder = new StringBuilder();

            builder.Append(characters.Select(c => (char)c).ToArray());

            builder.Replace("\0", "");

            return builder.ToString();
        }
    }
}