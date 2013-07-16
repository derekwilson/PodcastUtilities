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
using System.Linq;
using System.Xml;
using NUnit.Framework;
using PodcastUtilities.Common.Configuration;

namespace PodcastUtilities.Common.Tests.Configuration.ControlFileTests.Creation
{
    abstract class WhenCreatingAControlfileWithDifferentEpisodeDownloadStrategies : WhenTestingAControlFile
    {
        protected string StrategyText { get; set; }
        protected PodcastEpisodeDownloadStrategy Strategy1 { get; set; }
        protected PodcastEpisodeDownloadStrategy Strategy2 { get; set; }
        protected Exception ThrownException { get; set; }

        protected override void GivenThat()
        {
            base.GivenThat();

            XmlNode n = ControlFileXmlDocument.SelectSingleNode("podcasts/global/feed/downloadStrategy");
            n.InnerText = StrategyText;
        }

        protected override void When()
        {
            ThrownException = null;
            try
            {
                ControlFile = new ReadOnlyControlFile(ControlFileXmlDocument);

                // this will be the defaulted value from the global section
                Strategy1 = ControlFile.GetPodcasts().ElementAt(1).Feed.DownloadStrategy.Value;
                // this is set explicitly by the feed
                Strategy2 = ControlFile.GetPodcasts().ElementAt(2).Feed.DownloadStrategy.Value;
            }
            catch (Exception exception)
            {
                ThrownException = exception;
            }
        }
    }

    class WhenCreatingAControlFileWithNoDefaultEpisodeDownloadStrategy : WhenCreatingAControlfileWithDifferentEpisodeDownloadStrategies
    {
        protected override void GivenThat()
        {
            StrategyText = "";
            base.GivenThat();
        }

        [Test]
        public void ItShouldNotThorw()
        {
            Assert.That(ThrownException, Is.Null);
        }

        [Test]
        public void ItShouldReadTheGlobalFormat()
        {
            Assert.That(Strategy1, Is.EqualTo(PodcastEpisodeDownloadStrategy.All));
        }

        [Test]
        public void ItShouldReadTheSetFormat()
        {
            Assert.That(Strategy2, Is.EqualTo(PodcastEpisodeDownloadStrategy.HighTide));
        }
    }

    class WhenCreatingAControlFileWithDefaultEpisodeDownloadStrategy : WhenCreatingAControlfileWithDifferentEpisodeDownloadStrategies
    {
        protected override void GivenThat()
        {
            StrategyText = "latest";
            base.GivenThat();
        }

        [Test]
        public void ItShouldNotThorw()
        {
            Assert.That(ThrownException, Is.Null);
        }

        [Test]
        public void ItShouldReadTheFormat()
        {
            Assert.That(Strategy1, Is.EqualTo(PodcastEpisodeDownloadStrategy.Latest));
            Assert.That(Strategy2, Is.EqualTo(PodcastEpisodeDownloadStrategy.HighTide));
        }
    }


}