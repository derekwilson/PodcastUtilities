using System;
using System.Collections.Generic;
using System.Linq;
using PortableDeviceApiLib;

namespace PodcastUtilities.PortableDevices
{
    internal class DeviceObject : IDeviceObject
    {
        private readonly IPortableDeviceHelper _portableDeviceHelper;
        private readonly IPortableDeviceContent _portableDeviceContent;

        public DeviceObject(
            IPortableDeviceHelper portableDeviceHelper, 
            IPortableDeviceContent portableDeviceContent, 
            string id, 
            string name)
        {
            _portableDeviceHelper = portableDeviceHelper;
            _portableDeviceContent = portableDeviceContent;
            Id = id;
            Name = name;
        }

        public string Id { get; private set; }
        public string Name { get; private set; }

        public IEnumerable<IDeviceObject> GetFolders(string pattern)
        {
            // TODO: caching

            var childObjectIds = GetFilteredChildObjectIds(IsFolder);

            return childObjectIds.Select(CreateDeviceObject);
        }

        public IEnumerable<IDeviceObject> GetFiles(string pattern)
        {
            // TODO: caching

            var childObjectIds = GetFilteredChildObjectIds(IsFile);

            return childObjectIds.Select(CreateDeviceObject);
        }

        private IEnumerable<string> GetFilteredChildObjectIds(Predicate<string> match)
        {
            return _portableDeviceHelper.GetChildObjectIds(_portableDeviceContent, Id).Where(id => match(id));
        }

        private bool IsFolder(string objectId)
        {
            var contentType = _portableDeviceHelper.GetObjectContentType(_portableDeviceContent, objectId);
            return ((contentType == PortableDeviceConstants.WPD_CONTENT_TYPE_FOLDER) ||
                    (contentType == PortableDeviceConstants.WPD_CONTENT_TYPE_FUNCTIONAL_OBJECT));
        }

        private bool IsFile(string objectId)
        {
            return !IsFolder(objectId);
        }

        private IDeviceObject CreateDeviceObject(string objectId)
        {
            var objectName = _portableDeviceHelper.GetObjectName(_portableDeviceContent, objectId);

            return new DeviceObject(_portableDeviceHelper, _portableDeviceContent, objectId, objectName);
        }
    }
}