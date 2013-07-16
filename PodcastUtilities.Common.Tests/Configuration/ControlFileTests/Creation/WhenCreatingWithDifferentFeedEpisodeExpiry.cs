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
    public abstract class WhenCreatingAControlfileWithDifferentFeedEpisodeExpiry : WhenTestingAControlFile
    {
        protected string FeedMaximumDaysOldText { get; set; }
        protected int MaxDaysOld1 { get; set; }
        protected int MaxDaysOld2 { get; set; }
        protected Exception ThrownException { get; set; }

        protected override void GivenThat()
        {
            base.GivenThat();

            XmlNode n = ControlFileXmlDocument.SelectSingleNode("podcasts/global/feed/maximumDaysOld");
            n.InnerText = FeedMaximumDaysOldText;
        }

        protected override void When()
        {
            ThrownException = null;
            try
            {
                ControlFile = new ReadOnlyControlFile(ControlFileXmlDocument);

                // this will be the defaulted value from the global section
                MaxDaysOld1 = ControlFile.GetPodcasts().ElementAt(1).Feed.MaximumDaysOld.Value;
                // this is set explicitly by the feed
                MaxDaysOld2 = ControlFile.GetPodcasts().ElementAt(2).Feed.MaximumDaysOld.Value;
            }
            catch (Exception exception)
            {
                ThrownException = exception;
            }
        }
    }

    public class WhenCreatingAControlFileWithNoDefaultMaxDaysOld : WhenCreatingAControlfileWithDifferentFeedEpisodeExpiry
    {
        protected override void GivenThat()
        {
            FeedMaximumDaysOldText = "";
            base.GivenThat();
        }

        [Test]
        public void ItShouldNotThorw()
        {
            Assert.That(ThrownException, Is.Null);
        }

        [Test]
        public void ItShouldReadTheValue()
        {
            Assert.That(MaxDaysOld1, Is.EqualTo(int.MaxValue));
            Assert.That(MaxDaysOld2, Is.EqualTo(11));
        }
    }

    public class WhenCreatingAControlFileWithDefaultMaxDaysOld : WhenCreatingAControlfileWithDifferentFeedEpisodeExpiry
    {
        protected override void GivenThat()
        {
            FeedMaximumDaysOldText = "88";
            base.GivenThat();
        }

        [Test]
        public void ItShouldNotThorw()
        {
            Assert.That(ThrownException, Is.Null);
        }

        [Test]
        public void ItShouldReadTheValue()
        {
            Assert.That(MaxDaysOld1, Is.EqualTo(88));
            Assert.That(MaxDaysOld2, Is.EqualTo(11));
        }
    }
}

