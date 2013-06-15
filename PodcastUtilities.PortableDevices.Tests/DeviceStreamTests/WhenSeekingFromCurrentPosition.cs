using System;
using System.IO;
using NUnit.Framework;
using Rhino.Mocks;

namespace PodcastUtilities.PortableDevices.Tests.DeviceStreamTests
{
    public class WhenSeekingFromCurrentPosition : WhenTestingDeviceStream
    {
        protected long NewPosition { get; set; }

        protected override void When()
        {
            NewPosition = DeviceStream.Seek(345, SeekOrigin.Current);
        }

        [Test]
        public void ItShouldSeekUnderlyingStreamFromCurrentPosition()
        {
            Stream.AssertWasCalled(
                stream => stream.Seek(
                    Arg<long>.Is.Equal(345L), 
                    Arg<int>.Is.Equal(StreamConstants.STREAM_SEEK_CUR),
                    Arg<IntPtr>.Is.Anything));
        }

        [Test]
        public void ItShouldReturnNewPosition()
        {
            Assert.That(NewPosition, Is.EqualTo(1234L));
        }
    }
}