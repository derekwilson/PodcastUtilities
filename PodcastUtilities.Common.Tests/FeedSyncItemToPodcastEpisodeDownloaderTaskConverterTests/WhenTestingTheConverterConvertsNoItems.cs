using NUnit.Framework;

namespace PodcastUtilities.Common.Tests.FeedSyncItemToPodcastEpisodeDownloaderTaskConverterTests
{
    public class WhenTestingTheConverterConvertsNoItems : WhenTestingTheConverter
    {
        protected override void When()
        {
            _tasks = _converter.ConvertItemsToTasks(_downloadItems, null, null);
        }

        [Test]
        public void ItShouldReturnTheTasks()
        {
            Assert.That(_tasks.Length, Is.EqualTo(0));
        }
    }
}