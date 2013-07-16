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
using PodcastUtilities.Common.Feeds;

namespace PodcastUtilities.Common.Tests.Feeds.SyncItemToEpisodeDownloaderTaskConverterTests
{
    public class WhenTestingTheConverterConvertsItems : WhenTestingTheConverter
    {
        protected override void SetupData()
        {
            base.SetupData();
            _downloadItems.Add(new SyncItem()
                                   {
                                       DestinationPath = "destination1",
                                       EpisodeTitle = "item1",
                                       EpisodeUrl = new Uri("http://test1")
                                   });
            _downloadItems.Add(new SyncItem()
                                   {
                                       DestinationPath = "destination2",
                                       EpisodeTitle = "item2",
                                       EpisodeUrl = new Uri("http://test2")
                                   });
        }

        protected override void When()
        {
            _tasks = _converter.ConvertItemsToTasks(_downloadItems, null, null);
        }

        [Test]
        public void ItShouldReturnTheCorrectNumberOfTasks()
        {
            Assert.That(_tasks.Length, Is.EqualTo(2));
        }

        [Test]
        public void ItShouldReturnTheCorrectTypes()
        {
            Assert.IsInstanceOf(typeof(ITask), _tasks[0]);
            Assert.IsInstanceOf(typeof(ITask), _tasks[1]);
        }

        [Test]
        public void ItShouldReturnTasks0()
        {
            Assert.That(_tasks[0].SyncItem.DestinationPath, Is.EqualTo("destination1"));
            Assert.That(_tasks[0].SyncItem.EpisodeTitle, Is.EqualTo("item1"));
            Assert.That(_tasks[0].SyncItem.EpisodeUrl.ToString(), Is.EqualTo("http://test1/"));
        }

        [Test]
        public void ItShouldReturnTasks1()
        {
            Assert.That(_tasks[1].SyncItem.DestinationPath, Is.EqualTo("destination2"));
            Assert.That(_tasks[1].SyncItem.EpisodeTitle, Is.EqualTo("item2"));
            Assert.That(_tasks[1].SyncItem.EpisodeUrl.ToString(), Is.EqualTo("http://test2/"));
        }
    }
}