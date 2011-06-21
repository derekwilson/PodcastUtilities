using System;
using System.Linq;
using System.Threading;

namespace PodcastUtilities.Common
{
    /// <summary>
    /// a pool of tasks to be performed
    /// used in preference to the ThreadPool as we need long running tasks
    /// </summary>
    public class TaskPool : ITaskPool
    {
        private object _lock = new object();
        private ITask[] _tasks;

        /// <summary>
        /// run all the tasks in the pool
        /// </summary>
        /// <param name="numberOfTHreads">number of background threads to use</param>
        /// <param name="tasks">tasks to run</param>
        public void RunAllTasks(int numberOfTHreads, ITask[] tasks)
        {
            lock (_lock)
            {
                _tasks = tasks;
            }

            var runningTasks = StartTasks(numberOfTHreads);

            while (runningTasks.Length > 0)
            {
                WaitHandle.WaitAny(runningTasks);

                // start the next task in the pool
                runningTasks = StartTasks(1);
            }
        }

        /// <summary>
        /// abandon all the incomplete and instarted tasks in the pool
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
            foreach (var task in _tasks)
            {
                if (!task.IsStarted() && !task.IsComplete())
                {
                    task.Start(null);
                    numberOfTasks--;
                    if (numberOfTasks == 0)
                    {
                        break;
                    }
                }
            }
            var runningTasks =
                (from task in _tasks
                 where task.IsStarted() && !task.IsComplete()
                 select task.TaskComplete).ToArray();
            return runningTasks;
        }
    }
}
