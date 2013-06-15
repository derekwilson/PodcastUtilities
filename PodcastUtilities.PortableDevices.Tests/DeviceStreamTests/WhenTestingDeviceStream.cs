using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using PodcastUtilities.Common.Tests;
using Rhino.Mocks;

namespace PodcastUtilities.PortableDevices.Tests.DeviceStreamTests
{
    public abstract class WhenTestingDeviceStream
        : WhenTestingBehaviour
    {
        protected DeviceStream DeviceStream { get; set; }
        protected IStream Stream { get; set; }

        protected override void GivenThat()
        {
            base.GivenThat();

            Stream = GenerateMock<IStream>();

            // Make seek return a non-zero position
            Stream.Stub(stream => stream.Seek(0, 0, IntPtr.Zero))
                .IgnoreArguments()
                .WhenCalled(invocation => Marshal.WriteInt64((IntPtr) invocation.Arguments[2], 1234));

            DeviceStream = new DeviceStream(Stream);
        }
    }
}