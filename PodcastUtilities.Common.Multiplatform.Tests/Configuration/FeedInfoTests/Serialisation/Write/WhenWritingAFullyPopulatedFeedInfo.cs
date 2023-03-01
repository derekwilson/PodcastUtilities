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
using NUnit.Framework;
using PodcastUtilities.Common.Configuration;

namespace PodcastUtilities.Common.Multiplatform.Tests.Configuration.FeedInfoTests.Serialisation.Write
{
    public class WhenWritingAFullyPopulatedFeedInfo : WhenWritingAFeedInfo
    {
        protected override void GivenThat()
        {
            base.GivenThat();
            _feedInfo = new FeedInfo(_controlFile)
            {
                Address = new Uri("http://test.com"),
            };
            _feedInfo.MaximumNumberOfDownloadedItems.Value = 123;
            _feedInfo.DeleteDownloadsDaysOld.Value = 456;
            _feedInfo.DownloadStrategy.Value = PodcastEpisodeDownloadStrategy.HighTide;
            _feedInfo.Format.Value = PodcastFeedFormat.RSS;
            _feedInfo.MaximumDaysOld.Value = 789;
            _feedInfo.NamingStyle.Value = PodcastEpisodeNamingStyle.UrlFileName;
        }

        protected override void When()
        {
            _feedInfo.WriteXml(_xmlWriter);
            base.When();
        }

        [Test]
        public void ItShouldWriteTheXml()
        {
            Assert.That(_textReader.ReadToEnd(), Is.EqualTo("<url>http://test.com/</url><downloadStrategy>high_tide</downloadStrategy><format>rss</format><maximumDaysOld>789</maximumDaysOld><namingStyle>url</namingStyle><deleteDownloadsDaysOld>456</deleteDownloadsDaysOld><maximumNumberOfDownloadedItems>123</maximumNumberOfDownloadedItems>"));
        }
    }
}