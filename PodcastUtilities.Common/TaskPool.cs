using System.Collections.Generic;
using System.Threading;

namespace PodcastUtilities.Common
{
    /// <summary>
    /// a pool of tasks to be performed
    /// used in preference to the ThreadPool as we need long running tasks
    /// </summary>
    public class TaskPool : ITaskPool
    {
        private readonly object _lock = new object();
        private ITask[] _tasks;

        /// <summary>
        /// run all the tasks in the pool
        /// </summary>
        /// <param name="numberOfThreads">number of background threads to use</param>
        /// <param name="tasks">tasks to run</param>
        public void RunAllTasks(int numberOfThreads, ITask[] tasks)
        {
            lock (_lock)
            {
                _tasks = tasks;
            }

			var currentlyRunningTasks = new List<EventWaitHandle>(StartTasks(numberOfThreads));

			while (currentlyRunningTasks.Count > 0)
			{
				var completedIndex = WaitHandle.WaitAny(currentlyRunningTasks.ToArray());

				currentlyRunningTasks.RemoveAt(completedIndex);

				var newTasks = StartTasks(1);

				currentlyRunningTasks.AddRange(newTasks);
			}
        }

        /// <summary>
        /// abandon all the incomplete and unstarted tasks in the pool
        /// </summary>
        public void CancelAllTasks()
        {
            lock (_lock)
            {
                if (_tasks == null)
                {
                    return;
                }

                foreach (var task in _tasks)
                {
                    task.Cancel();
                }
            }
        }

        private EventWaitHandle[] StartTasks(int numberOfTasks)
        {
        	var newStartedTasks = new List<EventWaitHandle>();

            foreach (var task in _tasks)
            {
                if (!task.IsStarted() && !task.IsComplete())
                {
                    task.Start(null);
					newStartedTasks.Add(task.TaskComplete);

                    numberOfTasks--;
                    if (numberOfTasks == 0)
                    {
                        break;
                    }
                }
            }

        	return newStartedTasks.ToArray();
        }
    }
}
