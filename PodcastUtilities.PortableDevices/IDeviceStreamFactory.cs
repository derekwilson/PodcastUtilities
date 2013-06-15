using System;
using System.IO;
using PortableDeviceApiLib;

namespace PodcastUtilities.PortableDevices
{
    [CLSCompliant(false)]
    public interface IDeviceStreamFactory
    {
        Stream CreateStream(IStream deviceStream);
    }
}