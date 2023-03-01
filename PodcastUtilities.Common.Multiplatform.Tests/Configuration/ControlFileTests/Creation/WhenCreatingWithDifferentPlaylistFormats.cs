#region License
// FreeBSD License
// Copyright (c) 2010 - 2013, Andrew Trevarrow and Derek Wilson
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
// 
// Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
// 
// Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED 
// WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A 
// PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR
// ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED
// TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING 
// NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
// POSSIBILITY OF SUCH DAMAGE.
#endregion
using System;
using System.Xml;
using NUnit.Framework;
using PodcastUtilities.Common.Configuration;
using PodcastUtilities.Common.Playlists;

namespace PodcastUtilities.Common.Multiplatform.Tests.Configuration.ControlFileTests.Creation
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

        }

        protected override void When()
        {
            ThrownException = null;
            try
            {
                ControlFile = new ReadOnlyControlFile(ControlFileXmlDocument);
                Format = ControlFile.GetPlaylistFormat();
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

    class WhenCreatingAControlFileWithM3uPlaylistFormat : WhenCreatingAControlFileWithDifferentPlaylistFormats
    {
        protected override void GivenThat()
        {
            ControlFileFormatText = "M3U";
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
            Assert.That(Format, Is.EqualTo(PlaylistFormat.M3U));
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
        public void ItShouldNotThorw()
        {
            Assert.That(ThrownException, Is.Null);
        }

        [Test]
        public void ItShouldReadPlaylistFormat()
        {
            Assert.That(Format, Is.EqualTo(PlaylistFormat.Unknown));
        }
    }

    class WhenCreatingAControlFileWithAnInvalidPlaylistFormat : WhenCreatingAControlFileWithDifferentPlaylistFormats
    {
        protected override void GivenThat()
        {
            ControlFileFormatText = "INVALID";
            base.GivenThat();
        }

        [Test]
        public void ItShouldThorw()
        {
            Assert.That(ThrownException, Is.InstanceOf<NotSupportedException>());
        }
    }
}
