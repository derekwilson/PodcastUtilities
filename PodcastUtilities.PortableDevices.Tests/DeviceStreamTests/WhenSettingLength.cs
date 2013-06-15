using NUnit.Framework;
using Rhino.Mocks;

namespace PodcastUtilities.PortableDevices.Tests.DeviceStreamTests
{
    public class WhenSettingLength : WhenTestingDeviceStream
    {
        protected override void When()
        {
            DeviceStream.SetLength(9876L);
        }

        [Test]
        public void ItShouldSetSizeOnUnderlyingStream()
        {
            Stream.AssertWasCalled(
                stream => stream.SetSize(9876L));
        }
    }
}