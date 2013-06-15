using NUnit.Framework;
using Rhino.Mocks;

namespace PodcastUtilities.PortableDevices.Tests.DeviceStreamTests
{
    public class WhenFlushingDeviceStream : WhenTestingDeviceStream
    {
        protected override void When()
        {
            DeviceStream.Flush();
        }

        [Test]
        public void ItShouldCommitUnderlyingStream()
        {
            Stream.AssertWasCalled(stream => stream.Commit(0));
        }
    }
}