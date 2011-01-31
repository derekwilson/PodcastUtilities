using NUnit.Framework;
using Rhino.Mocks;

namespace PodcastUtilities.Common.Tests.PodcastSynchronizerTests
{
	public class WhenSubscribingToStatusUpdateEvent : WhenTestingPodcastSynchronizer
	{

		protected override void When()
		{
			PodcastSynchronizer.StatusUpdate += PodcastSynchronizerStatusUpdate;
		}

		static void PodcastSynchronizerStatusUpdate(object sender, StatusUpdateEventArgs e)
		{
		}

		[Test]
		public void ItShouldSubscribeToFileCopierStatusUpdate()
		{
			FileCopier.AssertWasCalled(c => c.StatusUpdate += PodcastSynchronizerStatusUpdate);
		}

		[Test]
		public void ItShouldSubscribeToFileRemoverStatusUpdate()
		{
			FileRemover.AssertWasCalled(c => c.StatusUpdate += PodcastSynchronizerStatusUpdate);
		}
	}
}