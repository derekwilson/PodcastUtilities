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
        /// <param name="numberOfThreads">number of background threads to use</param>
        /// <param name="tasks">tasks to run</param>
        void RunAllTasks(int numberOfThreads, ITask[] tasks);

        /// <summary>
        /// abandon all the incomplete and unstarted tasks in the pool
        /// </summary>
        void CancelAllTasks();
    }
}