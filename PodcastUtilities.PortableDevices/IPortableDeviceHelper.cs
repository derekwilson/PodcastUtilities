using System;
using System.Collections.Generic;
using PortableDeviceApiLib;

namespace PodcastUtilities.PortableDevices
{
    [CLSCompliant(false)]
    public interface IPortableDeviceHelper
    {
        string GetDeviceFriendlyName(IPortableDeviceManager portableDeviceManager, string deviceId);

        string GetObjectFileName(IPortableDeviceContent deviceContent, string objectId);
        string GetObjectName(IPortableDeviceContent deviceContent, string objectId);
        Guid GetObjectContentType(IPortableDeviceContent deviceContent, string objectId);
        DateTime GetObjectCreationTime(IPortableDeviceContent deviceContent, string objectId);

        string GetStringProperty(IPortableDeviceContent deviceContent, string objectId, _tagpropertykey key);
        Guid GetGuidProperty(IPortableDeviceContent deviceContent, string objectId, _tagpropertykey key);

        IEnumerable<string> GetChildObjectIds(IPortableDeviceContent deviceContent, string parentId);

        void DeleteObject(IPortableDeviceContent deviceContent, string objectId);

        IStream OpenResourceStream(IPortableDeviceContent deviceContent, string objectId, uint mode);
    }
}