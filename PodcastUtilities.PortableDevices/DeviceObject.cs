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
        private readonly IFilenameMatcher _filenameMatcher;

        public DeviceObject(
            IPortableDeviceHelper portableDeviceHelper, 
            IPortableDeviceContent portableDeviceContent, 
            string id, 
            string name)
            : this(portableDeviceHelper, portableDeviceContent, new FilenameMatcher(), id, name)
        {
        }

        public DeviceObject(
            IPortableDeviceHelper portableDeviceHelper, 
            IPortableDeviceContent portableDeviceContent, 
            IFilenameMatcher filenameMatcher,
            string id, 
            string name)
        {
            _portableDeviceHelper = portableDeviceHelper;
            _portableDeviceContent = portableDeviceContent;
            _filenameMatcher = filenameMatcher;
            Id = id;
            Name = name;
        }

        public string Id { get; private set; }
        public string Name { get; private set; }

        public IEnumerable<IDeviceObject> GetFolders(string pattern)
        {
            // TODO: caching

            var childObjectIds = GetFilteredChildObjectIds(id => IsFolder(id) && IsObjectFilenameMatch(id, pattern));

            return childObjectIds.Select(CreateDeviceObject);
        }

        public IEnumerable<IDeviceObject> GetFiles(string pattern)
        {
            // TODO: caching

            var childObjectIds = GetFilteredChildObjectIds(id => IsFile(id) && IsObjectFilenameMatch(id, pattern));

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

        private bool IsObjectFilenameMatch(string id, string pattern)
        {
            var filename = _portableDeviceHelper.GetObjectFileName(_portableDeviceContent, id);

            return _filenameMatcher.IsMatch(filename, pattern);
        }

        private IDeviceObject CreateDeviceObject(string objectId)
        {
            var objectName = _portableDeviceHelper.GetObjectFileName(_portableDeviceContent, objectId);

            return new DeviceObject(_portableDeviceHelper, _portableDeviceContent, objectId, objectName);
        }
    }
}