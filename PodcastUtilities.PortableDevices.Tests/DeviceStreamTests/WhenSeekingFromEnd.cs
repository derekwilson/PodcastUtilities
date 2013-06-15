using System;
using System.IO;
using NUnit.Framework;
using Rhino.Mocks;

namespace PodcastUtilities.PortableDevices.Tests.DeviceStreamTests
{
    public class WhenSeekingFromEnd : WhenTestingDeviceStream
    {
        protected long NewPosition { get; set; }

        protected override void When()
        {
            NewPosition = DeviceStream.Seek(234, SeekOrigin.End);
        }

        [Test]
        public void ItShouldSeekUnderlyingStreamFromEnd()
        {
            Stream.AssertWasCalled(
                stream => stream.Seek(
                    Arg<long>.Is.Equal(234L), 
                    Arg<int>.Is.Equal(StreamConstants.STREAM_SEEK_END),
                    Arg<IntPtr>.Is.Anything));
        }

        [Test]
        public void ItShouldReturnNewPosition()
        {
            Assert.That(NewPosition, Is.EqualTo(1234L));
        }
    }
}