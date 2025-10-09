using FakeItEasy;
using PodcastUtilities.Common.Feeds;
using System;

namespace PodcastUtilities.AndroidTests.Helpers
{
    public class SyncItemMocker
    {
        private ISyncItem MockSyncItem = A.Fake<ISyncItem>();

        public ISyncItem GetMockedSyncItem() => MockSyncItem;

        public SyncItemMocker ApplyId(Guid id)
        {
            A.CallTo(() => MockSyncItem.Id).Returns(id);
            return this;
        }
        public SyncItemMocker ApplyEpisodeTitle(string title)
        {
            A.CallTo(() => MockSyncItem.EpisodeTitle).Returns(title);
            return this;
        }
        public SyncItemMocker ApplyPublished(DateTime published)
        {
            A.CallTo(() => MockSyncItem.Published).Returns(published);
            return this;
        }
    }
}