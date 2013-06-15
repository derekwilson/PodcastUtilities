using System.Linq;
using NUnit.Framework;

namespace PodcastUtilities.PortableDevices.Tests.DeviceStreamTests
{
    public class WhenReadingFromStreamOffsetIntoBuffer : WhenReadingFromStream
    {
        protected override void When()
        {
            ReadCount = DeviceStream.Read(ReadBuffer, 10, 5);
        }

        [Test]
        public void ItShouldReadToTheCorrectBufferPosition()
        {
            Assert.That(ReadBuffer.Take(10).All(b => b == 0));
            Assert.That(ReadBuffer.Skip(10).Take(4).All(b => b == 99));
            Assert.That(ReadBuffer.Skip(14).Take(6).All(b => b == 0));
        }

        [Test]
        public void ItShouldReturnTheActualNumberRead()
        {
            Assert.That(ReadCount, Is.EqualTo(4));
        }
    }
}