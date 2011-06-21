namespace PodcastUtilities.Common
{
    /// <summary>
    /// a pool of tasks to be performed
    /// used in preference to the ThreadPool as we need long running tasks
    /// </summary>
    public interface ITaskPool
    {
        /// <summary>
        /// run all the tasks in the pool
        /// </summary>
        /// <param name="numberOfTHreads">number of background threads to use</param>
        /// <param name="tasks">tasks to run</param>
        void RunAllTasks(int numberOfTHreads, ITask[] tasks);

        /// <summary>
        /// abandon all the incomplete and instarted tasks in the pool
        /// </summary>
        void CancelAllTasks();
    }
}