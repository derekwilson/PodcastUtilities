using System;
using System.Threading;

namespace PodcastUtilities.Common.Tests.TaskPoolTests
{
	public class TestTask
		: ITask
	{
		private readonly string _name;
		private bool _started;
		private bool _complete;

		public TestTask(string name)
		{
			_name = name;

			TaskComplete = new ManualResetEvent(false);
			Started = new ManualResetEvent(false);

			Console.WriteLine("Task event handle: {0}", TaskComplete.SafeWaitHandle.DangerousGetHandle());
		}

		public EventWaitHandle Started { get; private set; }

		#region Implementation of IStatusUpdate

		public event EventHandler<StatusUpdateEventArgs> StatusUpdate
		{
			add { }
			remove { }
		}

		#endregion

		#region Implementation of IProgressUpdate

		public event EventHandler<ProgressEventArgs> ProgressUpdate
		{
			add { }
			remove { }
		}

		#endregion

		public void ForceComplete()
		{
			_complete = true;
			TaskComplete.Set();
		}

		#region Implementation of ITask

		public EventWaitHandle TaskComplete { get; private set; }

		public string GetName()
		{
			return _name;
		}

		public void Start(object state)
		{
			_started = true;
			Started.Set();
		}

		public void Cancel()
		{
			ForceComplete();
		}

		public bool IsStarted()
		{
			return _started;
		}

		public bool IsComplete()
		{
			return _complete;
		}

		#endregion
	}
}
