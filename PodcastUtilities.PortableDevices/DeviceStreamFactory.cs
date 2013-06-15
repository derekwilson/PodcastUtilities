using System;
using System.IO;
using PortableDeviceApiLib;

namespace PodcastUtilities.PortableDevices
{
    [CLSCompliant(false)]
    public class DeviceStreamFactory : IDeviceStreamFactory
    {
        public Stream CreateStream(IStream deviceStream)
        {
            return new DeviceStream(deviceStream);
        }
    }
}