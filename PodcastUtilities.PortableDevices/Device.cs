using System;
using System.IO;
using PortableDeviceApiLib;

namespace PodcastUtilities.PortableDevices
{
    public class Device : IDevice
    {
        private readonly IPortableDeviceManager _portableDeviceManager;
        private readonly IPortableDeviceFactory _portableDeviceFactory;
        private readonly IPortableDeviceHelper _portableDeviceHelper;
        private readonly IDeviceStreamFactory _deviceStreamFactory;
        private IPortableDevice _portableDevice;
        private IPortableDeviceContent _portableDeviceContent;
        private string _name;

        internal Device(
            IPortableDeviceManager portableDeviceManager,
            IPortableDeviceFactory portableDeviceFactory, 
            IPortableDeviceHelper portableDeviceHelper,
            IDeviceStreamFactory deviceStreamFactory,
            string id)
        {
            _portableDeviceManager = portableDeviceManager;
            _portableDeviceFactory = portableDeviceFactory;
            _portableDeviceHelper = portableDeviceHelper;
            _deviceStreamFactory = deviceStreamFactory;
            Id = id;
            OpenDevice(id);
        }

        public string Id { get; private set; }

        public string Name
        {
            get { return _name ?? (_name = GetDeviceName()); }
        }

        public IDeviceObject GetObjectFromPath(string path)
        {
            var pathParts = path.Split(Path.DirectorySeparatorChar);

            var parentId = PortableDeviceConstants.WPD_DEVICE_OBJECT_ID;
            string childObjectId = null;
            string childObjectName = null;

            foreach (var part in pathParts)
            {
                childObjectId = GetChildObjectIdByName(parentId, part, out childObjectName);
                if (childObjectId == null)
                {
                    return null;
                }

                parentId = childObjectId;
            }

            return new DeviceObject(_portableDeviceHelper, _portableDeviceContent, childObjectId, childObjectName);
        }

        public void Delete(string path)
        {
            var deviceObject = GetObjectFromPath(path);
            if (deviceObject == null)
            {
                // As File.Delete(), do not throw an exception if the file doesn't exist
                return;
            }

            _portableDeviceHelper.DeleteObject(_portableDeviceContent, deviceObject.Id);
        }

        public Stream OpenRead(string path)
        {
            var deviceObject = GetObjectFromPath(path);
            if (deviceObject == null)
            {
                throw new FileNotFoundException(String.Format("Device: [{0}]; path [{1}] not found", Name, path));
            }

            var resourceStream = _portableDeviceHelper.OpenResourceStream(
                _portableDeviceContent, 
                deviceObject.Id, 
                StreamConstants.STGM_READ);

            return _deviceStreamFactory.CreateStream(resourceStream);
        }

        public Stream OpenWrite(string path, bool allowOverwrite)
        {
            throw new NotImplementedException();
        }

        private void OpenDevice(string id)
        {
            _portableDevice = _portableDeviceFactory.Create(id);
            _portableDevice.Content(out _portableDeviceContent);
        }

        private string GetDeviceName()
        {
            // _portableDeviceManager.GetDeviceFriendlyName() doesn't work for some devices (eg. my HTC One S)
            var name = _portableDeviceHelper.GetDeviceFriendlyName(_portableDeviceManager, Id);
            if (name != null)
            {
                return name;
            }
            
            return _portableDeviceHelper.GetObjectFileName(
                _portableDeviceContent,
                PortableDeviceConstants.WPD_DEVICE_OBJECT_ID);
        }

        private string GetChildObjectIdByName(string parentObjectId, string name, out string actualObjectName)
        {
            var childObjectIds = _portableDeviceHelper.GetChildObjectIds(_portableDeviceContent, parentObjectId);

            foreach (var id in childObjectIds)
            {
                var childObjectName = _portableDeviceHelper.GetObjectFileName(
                    _portableDeviceContent, 
                    id);
                if (string.Compare(childObjectName, name, true) == 0)
                {
                    actualObjectName = childObjectName;
                    return id;
                }
            }

            actualObjectName = null;
            return null;
        }
    }
}