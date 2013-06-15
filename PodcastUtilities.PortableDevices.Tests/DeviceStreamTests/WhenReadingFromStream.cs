using System;
using System.Runtime.InteropServices;
using Rhino.Mocks;

namespace PodcastUtilities.PortableDevices.Tests.DeviceStreamTests
{
    public abstract class WhenReadingFromStream : WhenTestingDeviceStream
    {
        protected byte[] ReadBuffer { get; set; }
        protected int ReadCount { get; set; }

        protected override void GivenThat()
        {
            base.GivenThat();

            ReadBuffer = new byte[20];

            Stream.Stub(stream => stream.Read(null, 0, IntPtr.Zero))
                .IgnoreArguments()
                .WhenCalled(invocation => MockRead((byte[]) invocation.Arguments[0], (int) invocation.Arguments[1], (IntPtr) invocation.Arguments[2]));
        }

        private static void MockRead(byte[] buffer, int count, IntPtr countPointer)
        {
            // Simulate reading less than requested
            int available = count - 1;
            for (int i = 0; i < available; i++)
            {
                buffer[i] = 99;
            }

            Marshal.WriteInt32(countPointer, available);
        }
    }
}