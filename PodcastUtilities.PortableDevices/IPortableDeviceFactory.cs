using System;
using PortableDeviceApiLib;

namespace PodcastUtilities.PortableDevices
{
    [CLSCompliant(false)]
    public interface IPortableDeviceFactory
    {
        IPortableDevice Create(string deviceId);
    }
}