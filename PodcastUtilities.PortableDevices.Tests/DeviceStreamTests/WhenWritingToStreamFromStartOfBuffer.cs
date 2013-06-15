using NUnit.Framework;

namespace PodcastUtilities.PortableDevices.Tests.DeviceStreamTests
{
    public class WhenWritingToStreamFromStartOfBuffer : WhenWritingToStream
    {
        protected override void When()
        {
            DeviceStream.Write(BufferToWrite, 0, 5);
        }

        [Test]
        public void ItShouldWriteFromTheCorrectBufferPosition()
        {
            Assert.That(WrittenBuffer.Length, Is.EqualTo(5));
            for (int i = 0; i < WrittenBuffer.Length; i++)
            {
                Assert.That(WrittenBuffer[i], Is.EqualTo(BufferToWrite[i]));
            }
        }
    }
}