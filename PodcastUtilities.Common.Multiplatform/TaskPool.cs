﻿#region License
// FreeBSD License
// Copyright (c) 2010 - 2013, Andrew Trevarrow and Derek Wilson
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
// 
// Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
// 
// Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED 
// WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A 
// PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR
// ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED
// TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING 
// NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
// POSSIBILITY OF SUCH DAMAGE.
#endregion
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
