using System;
using NUnit.Framework;

namespace PodcastUtilities.Common.Tests.FeedSyncItemToPodcastEpisodeDownloaderTaskConverterTests
{
    public class WhenTestingTheConverterConvertsItems : WhenTestingTheConverter
    {
        protected override void SetupData()
        {
            base.SetupData();
            _downloadItems.Add(new FeedSyncItem()
                                   {
                                       DestinationPath = "destination1",
                                       EpisodeTitle = "item1",
                                       EpisodeUrl = new Uri("http://test1")
                                   });
            _downloadItems.Add(new FeedSyncItem()
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
        public void ItShouldReturnTheCorrectTypes()
        {
            Assert.That(_tasks.Length, Is.EqualTo(2));
            Assert.IsInstanceOf(typeof(ITask), _tasks[0]);
            Assert.IsInstanceOf(typeof(ITask), _tasks[1]);
        }

        [Test]
        public void ItShouldReturnTheTasks()
        {
            Assert.That(_tasks.Length, Is.EqualTo(2));
            Assert.That(_tasks[0].SyncItem.DestinationPath, Is.EqualTo("destination1"));
            Assert.That(_tasks[0].SyncItem.EpisodeTitle, Is.EqualTo("item1"));
            Assert.That(_tasks[0].SyncItem.EpisodeUrl.ToString(), Is.EqualTo("http://test1/"));
            Assert.That(_tasks[1].SyncItem.DestinationPath, Is.EqualTo("destination2"));
            Assert.That(_tasks[1].SyncItem.EpisodeTitle, Is.EqualTo("item2"));
            Assert.That(_tasks[1].SyncItem.EpisodeUrl.ToString(), Is.EqualTo("http://test2/"));
        }
    }
}