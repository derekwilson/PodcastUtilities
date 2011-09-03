using System;
using System.IO;
using System.Text;
using PodcastUtilities.Common.Feeds;

namespace PodcastUtilities.Common.Tests.Feeds.PodcastFeedFactoryTests
{
    public abstract class WhenCreatingANewPodcastFeed
        : WhenTestingBehaviour
    {
        protected IPodcastFeed Feed { get; set; }
        protected IPodcastFeedFactory FeedFactory { get; set; }
        protected Stream FeedData { get; set; }
        protected Exception ThrownException { get; set; }

        protected override void GivenThat()
        {
            base.GivenThat();

            FeedFactory = new PodcastFeedFactory();

            byte[] byteArray = Encoding.UTF8.GetBytes( "<xml></xml>" );
            FeedData = new MemoryStream( byteArray ); 
        }
    }
}
