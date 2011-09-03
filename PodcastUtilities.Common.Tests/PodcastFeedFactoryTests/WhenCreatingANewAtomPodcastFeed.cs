using System;
using NUnit.Framework;
using PodcastUtilities.Common.Configuration;
using PodcastUtilities.Common.Exceptions;

namespace PodcastUtilities.Common.Tests.PodcastFeedFactoryTests
{
    public class WhenCreatingANewAtomPodcastFeed : WhenCreatingANewPodcastFeed
    {
        protected override void When()
        {
            ThrownException = null;
            try
            {
                Feed = FeedFactory.CreatePodcastFeed(PodcastFeedFormat.ATOM, FeedData);
            }
            catch (Exception e)
            {
                ThrownException = e;
            }
        }

        [Test]
        public void ItShouldThrow()
        {
            Assert.That(ThrownException, Is.InstanceOf<EnumOutOfRangeException>());
        }
    }
}