using System.Runtime.InteropServices.ComTypes;
using NUnit.Framework;
using Rhino.Mocks;

namespace PodcastUtilities.PortableDevices.Tests.DeviceStreamTests
{
    public class WhenGettingLength : WhenTestingDeviceStream
    {
        protected override void GivenThat()
        {
            base.GivenThat();

            var streamStat = new STATSTG
                                 {
                                     cbSize = 55667788
                                 };
            Stream.Stub(stream => stream.Stat(out Arg<STATSTG>.Out(streamStat).Dummy, Arg<int>.Is.Equal(1)));
        }

        protected override void When()
        {
        }

        [Test]
        public void ItShouldGetSizeFromUnderlyingStream()
        {
            Assert.That(DeviceStream.Length, Is.EqualTo(55667788));
        }
    }
}