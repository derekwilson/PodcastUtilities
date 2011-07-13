using System.Linq;
using System.Threading;

namespace PodcastUtilities.Common.Tests.TaskPoolTests
{
	public abstract class WhenRunningTasks : WhenTestingTheTaskPool
	{
		protected TestTask[] Tasks { get; set; }

		protected Thread RunTasksThread { get; set; }

		protected abstract int NumberOfThreads { get; }

		protected override void GivenThat()
		{
			base.GivenThat();

			Tasks = new TestTask[]
			        	{
			        		new TestTask("1"),
			        		new TestTask("2"),
			        		new TestTask("3")
			        	};

			RunTasksThread = new Thread(() => TaskPool.RunAllTasks(NumberOfThreads, Tasks));

			RunTasksThread.Start();

			WaitHandle.WaitAll(Tasks.Select(t => t.Started).Take(NumberOfThreads).ToArray());
		}

		public override void CleanupAfterTest()
		{
			base.CleanupAfterTest();

			TaskPool.CancelAllTasks();
		}

	}
}