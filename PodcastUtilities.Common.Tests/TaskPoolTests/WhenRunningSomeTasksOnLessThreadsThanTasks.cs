using NUnit.Framework;

namespace PodcastUtilities.Common.Tests.TaskPoolTests
{
	public class WhenRunningSomeTasksOnLessThreadsThanTasks : WhenRunningTasks
	{
		protected override int NumberOfThreads
		{
			get { return 2; }
		}

		protected override void When()
		{
		}

		[Test]
		public void ItShouldHaveOnlyStartedTheRightNumberOfTasks()
		{
			Assert.That(Tasks[0].IsStarted(), Is.True);
			Assert.That(Tasks[1].IsStarted(), Is.True);
			Assert.That(Tasks[2].IsStarted(), Is.False);
		}
	}
}