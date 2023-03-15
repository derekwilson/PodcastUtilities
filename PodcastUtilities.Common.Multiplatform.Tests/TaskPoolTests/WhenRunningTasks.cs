#region License
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
using System.Linq;
using System.Threading;

namespace PodcastUtilities.Common.Multiplatform.Tests.TaskPoolTests
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