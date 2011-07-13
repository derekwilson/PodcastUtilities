using System;
using System.Xml;
using NUnit.Framework;

namespace PodcastUtilities.Common.Tests.ControlFileTests
{
    abstract class WhenCreatingAControlFileWithDifferentPlaylistFormats : WhenTestingAControlFile
    {
        protected string ControlFileFormatText { get; set; }
        protected PlaylistFormat Format { get; set; }
        protected Exception ThrownException { get; set; }

        protected override void GivenThat()
        {
            base.GivenThat();

            XmlNode n = ControlFileXmlDocument.SelectSingleNode("podcasts/global/playlistFormat");
            n.InnerText = ControlFileFormatText;

            ControlFile = new ControlFile(ControlFileXmlDocument);
        }

        protected override void When()
        {
            ThrownException = null;
            try
            {
                Format = ControlFile.PlaylistFormat;
            }
            catch (Exception exception)
            {
                ThrownException = exception;
            }
        }
    }

    class WhenCreatingAControlFileWithWplPlaylistFormat : WhenCreatingAControlFileWithDifferentPlaylistFormats
    {
        protected override void GivenThat()
        {
            ControlFileFormatText = "wpl";
            base.GivenThat();
        }

        [Test]
        public void ItShouldNotThorw()
        {
            Assert.That(ThrownException, Is.Null);
        }

        [Test]
        public void ItShouldReadPlaylistFormat()
        {
            Assert.That(Format, Is.EqualTo(PlaylistFormat.WPL));
        }
    }

    class WhenCreatingAControlFileWithAsxPlaylistFormat : WhenCreatingAControlFileWithDifferentPlaylistFormats
    {
        protected override void GivenThat()
        {
            ControlFileFormatText = "ASX";
            base.GivenThat();
        }

        [Test]
        public void ItShouldNotThorw()
        {
            Assert.That(ThrownException, Is.Null);
        }

        [Test]
        public void ItShouldReadPlaylistFormat()
        {
            Assert.That(Format, Is.EqualTo(PlaylistFormat.ASX));
        }
    }

    class WhenCreatingAControlFileWithAnUnknownPlaylistFormat : WhenCreatingAControlFileWithDifferentPlaylistFormats
    {
        protected override void GivenThat()
        {
            ControlFileFormatText = "UNKNOWN";
            base.GivenThat();
        }

        [Test]
        public void ItShouldThorw()
        {
            Assert.That(ThrownException, Is.InstanceOf<IndexOutOfRangeException>());
        }
    }
}
