using System.Threading;
using NUnit.Framework;

namespace PodcastUtilities.Common.Tests.TaskPoolTests
{
	public class WhenTheLastTaskCompletes : WhenRunningTasks
	{
		protected override int NumberOfThreads
		{
			get { return 2; }
		}

		protected override void GivenThat()
		{
			base.GivenThat();

			Tasks[0].ForceComplete();
			Tasks[1].ForceComplete();
			WaitHandle.WaitAll(new[] { Tasks[2].Started });
		}

		protected override void When()
		{
			Tasks[2].ForceComplete();
			RunTasksThread.Join();
		}

		[Test]
		public void ItShouldStopRunningTasks()
		{
			// If we reach here, the thread terminated ok

			Assert.That(Tasks[0].IsComplete());
			Assert.That(Tasks[1].IsComplete());
			Assert.That(Tasks[2].IsComplete());
		}
	}
}