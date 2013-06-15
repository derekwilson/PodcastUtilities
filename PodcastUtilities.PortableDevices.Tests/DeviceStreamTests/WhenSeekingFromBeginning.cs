using System;
using System.IO;
using NUnit.Framework;
using Rhino.Mocks;

namespace PodcastUtilities.PortableDevices.Tests.DeviceStreamTests
{
    public class WhenSeekingFromBeginning : WhenTestingDeviceStream
    {
        protected long NewPosition { get; set; }

        protected override void When()
        {
            NewPosition = DeviceStream.Seek(123, SeekOrigin.Begin);
        }

        [Test]
        public void ItShouldSeekUnderlyingStreamFromBeginning()
        {
            Stream.AssertWasCalled(
                stream => stream.Seek(
                    Arg<long>.Is.Equal(123L), 
                    Arg<int>.Is.Equal(StreamConstants.STREAM_SEEK_SET),
                    Arg<IntPtr>.Is.Anything));
        }

        [Test]
        public void ItShouldReturnNewPosition()
        {
            Assert.That(NewPosition, Is.EqualTo(1234L));
        }
    }
}