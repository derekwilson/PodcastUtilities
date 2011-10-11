using NUnit.Framework;
using PodcastUtilities.Common.Configuration;

namespace PodcastUtilities.Common.Tests.Configuration.PodcastInfoTests.Serialisation.Write
{
    public class WhenWritingAnUnpopulatedPodcastInfo : WhenWritingAPodcastInfo
    {
        protected override void GivenThat()
        {
            base.GivenThat();
            _podcastInfo = new PodcastInfo(_controlFile)
                               {
                                   Folder = "folder",
                                   MaximumNumberOfFiles = 123
                               };
        }

        protected override void When()
        {
            _podcastInfo.WriteXml(_xmlWriter);
            base.When();
        }

        [Test]
        public void ItShouldWriteTheXml()
        {
            Assert.That(_textReader.ReadToEnd(), Is.EqualTo("<folder>folder</folder><number>123</number>"));
        }
    }
}