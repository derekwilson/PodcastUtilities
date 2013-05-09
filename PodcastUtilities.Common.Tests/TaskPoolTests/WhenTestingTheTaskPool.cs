using System;
using NUnit.Framework;

namespace PodcastUtilities.Common.Tests.TaskPoolTests
{
	public abstract class WhenTestingTheTaskPool
		: WhenTestingBehaviour
	{
		public TaskPool TaskPool { get; set; }

		protected override void GivenThat()
		{
			base.GivenThat();

			TaskPool = new TaskPool();
		}
	}

	public class WhenCancellingTasksButNonStarted : WhenTestingTheTaskPool
	{
		public Exception CaughtException { get; set; }

		protected override void When()
		{
			try
			{
				TaskPool.CancelAllTasks();
			}
			catch (Exception ex)
			{
				CaughtException = ex;
			}
		}

		[Test]
		public void ItShouldHandleItWithoutThrowingException()
		{
			Assert.That(CaughtException, Is.Null);
		}
	}
}
