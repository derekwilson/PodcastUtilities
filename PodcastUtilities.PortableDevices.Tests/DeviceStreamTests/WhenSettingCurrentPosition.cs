using System;
using NUnit.Framework;
using Rhino.Mocks;

namespace PodcastUtilities.PortableDevices.Tests.DeviceStreamTests
{
    public class WhenSettingCurrentPosition : WhenTestingDeviceStream
    {
        protected override void When()
        {
            DeviceStream.Position = 7654;
        }

        [Test]
        public void ItShouldSeekUnderlyingStreamFromBeginning()
        {
            Stream.AssertWasCalled(
                stream => stream.Seek(
                    Arg<long>.Is.Equal(7654L),
                    Arg<int>.Is.Equal(StreamConstants.STREAM_SEEK_SET),
                    Arg<IntPtr>.Is.Anything));
        }
    }
}