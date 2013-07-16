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

	    public string Name
	    {
            get { return _name; }
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
