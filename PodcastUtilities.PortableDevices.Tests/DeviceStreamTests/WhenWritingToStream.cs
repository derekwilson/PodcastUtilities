using System;
using Rhino.Mocks;

namespace PodcastUtilities.PortableDevices.Tests.DeviceStreamTests
{
    public abstract class WhenWritingToStream : WhenTestingDeviceStream
    {
        protected byte[] BufferToWrite { get; set; }
        protected byte[] WrittenBuffer { get; set; }
        protected int ReadCount { get; set; }

        protected override void GivenThat()
        {
            base.GivenThat();

            BufferToWrite = new byte[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 10};

            Stream.Stub(stream => stream.Write(null, 0, IntPtr.Zero))
                .IgnoreArguments()
                .WhenCalled(invocation => MockWrite((byte[])invocation.Arguments[0], (int)invocation.Arguments[1], (IntPtr)invocation.Arguments[2]));
        }

        private void MockWrite(byte[] buffer, int count, IntPtr countPointer)
        {
            WrittenBuffer = new byte[count];
            Array.Copy(buffer, 0, WrittenBuffer, 0, count);
        }
    }
}