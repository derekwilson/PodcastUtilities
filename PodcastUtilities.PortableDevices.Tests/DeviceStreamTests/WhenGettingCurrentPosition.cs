using System;
using NUnit.Framework;
using Rhino.Mocks;

namespace PodcastUtilities.PortableDevices.Tests.DeviceStreamTests
{
    public class WhenGettingCurrentPosition : WhenTestingDeviceStream
    {
        protected long Position { get; set; }

        protected override void When()
        {
            Position = DeviceStream.Position;
        }

        [Test]
        public void ItShouldSeekUnderlyingStreamZeroOffsetFromCurrentPosition()
        {
            Stream.AssertWasCalled(
                stream => stream.Seek(
                    Arg<long>.Is.Equal(0L),
                    Arg<int>.Is.Equal(StreamConstants.STREAM_SEEK_CUR),
                    Arg<IntPtr>.Is.Anything));
        }

        [Test]
        public void ItShouldReturnThePositionFromSeek()
        {
            Assert.That(Position, Is.EqualTo(1234L));
        }
    }
}