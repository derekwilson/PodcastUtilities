using System.Linq;
using NUnit.Framework;

namespace PodcastUtilities.PortableDevices.Tests.DeviceStreamTests
{
    public class WhenReadingFromStreamToStartOfBuffer : WhenReadingFromStream
    {
        protected override void When()
        {
            ReadCount = DeviceStream.Read(ReadBuffer, 0, 10);
        }

        [Test]
        public void ItShouldReadToTheCorrectBufferPosition()
        {
            Assert.That(ReadBuffer.Take(9).All(b => b == 99));
            Assert.That(ReadBuffer.Skip(9).Take(11).All(b => b == 0));
        }

        [Test]
        public void ItShouldReturnTheActualNumberRead()
        {
            Assert.That(ReadCount, Is.EqualTo(9));
        }
    }
}