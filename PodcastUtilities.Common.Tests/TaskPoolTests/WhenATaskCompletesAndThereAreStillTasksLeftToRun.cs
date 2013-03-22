using System.Threading;
using NUnit.Framework;

namespace PodcastUtilities.Common.Tests.TaskPoolTests
{
	public class WhenATaskCompletesAndThereAreStillTasksLeftToRun : WhenRunningTasks
	{
		protected override int NumberOfThreads
		{
			get { return 2; }
		}

		protected override void When()
		{
			Tasks[1].ForceComplete();
			Tasks[2].Started.WaitOne();
		}

		[Test]
		public void ItShouldStartTheNextTask()
		{
			Assert.That(Tasks[2].IsStarted());
		}
	}
}