using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace PodcastUtilities.Common
{
    /// <summary>
    /// a task that can be performed on a background thread
    /// </summary>
    public interface ITask : IStatusUpdate, IProgressUpdate
    {
        /// <summary>
        /// the event that is fired when the task completes
        /// </summary>
        EventWaitHandle TaskComplete { get; }
        
        /// <summary>
        /// gets the display name for the task
        /// </summary>
        /// <returns></returns>
        string GetName();
        
        /// <summary>
        /// start running the task - the task is started in the background and the method will return
        /// </summary>
        void Start(object state);
        
        /// <summary>
        /// cancel a background task - or prevent a task from running
        /// </summary>
        void Cancel();
        
        /// <summary>
        /// true if the task has been started
        /// </summary>
        bool IsStarted();

        /// <summary>
        /// true if the task has been completed - or canceled
        /// </summary>
        bool IsComplete();
    }
}
