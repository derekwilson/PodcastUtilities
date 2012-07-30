using NUnit.Framework;
using PodcastUtilities.Common.Configuration;

namespace PodcastUtilities.Common.Tests.Configuration.PodcastInfoTests.Serialisation.Write
{
    public class WhenWritingAPopulatedPodcastInfo : WhenWritingAPodcastInfo
    {
        protected override void GivenThat()
        {
            base.GivenThat();
            _podcastInfo = new PodcastInfo(_controlFile)
                               {
                                   Folder = "folder"
                               };
            _podcastInfo.Pattern.Value = "pattern";
            _podcastInfo.SortField.Value = PodcastFileSortField.CreationTime;
            _podcastInfo.AscendingSort.Value = true;
            _podcastInfo.MaximumNumberOfFiles.Value = 123;
            _podcastInfo.PostDownloadCommand.Value = "postdownloadcommand";
        }

        protected override void When()
        {
            _podcastInfo.WriteXml(_xmlWriter);
            base.When();
        }

        [Test]
        public void ItShouldWriteTheXml()
        {
            Assert.That(_textReader.ReadToEnd(), Is.EqualTo("<folder>folder</folder><pattern>pattern</pattern><number>123</number><sortfield>creationtime</sortfield><sortdirection>asc</sortdirection><postdownloadcommand>postdownloadcommand</postdownloadcommand>"));
        }
    }
}