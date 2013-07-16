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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PodcastUtilities.Common.Configuration;
using PodcastUtilities.Ioc;

namespace PodcastUtilities.Integration.Tests
{
    enum DisplayLevel
    {
        Error,
        Warning,
        Title,
        Message,
        Verbose
    }

    abstract class RunnerBase : IRunner
    {
        static private object _synclock = new object();
        protected static LinFuIocContainer _iocContainer;
        protected bool _verbose = false;
        protected string _testsToRun = null;

        public abstract void RunAllTests();

        private static LinFuIocContainer InitializeIocContainer()
        {
            var container = new LinFuIocContainer();

            IocRegistration.RegisterSystemServices(container);
            IocRegistration.RegisterFileServices(container);
            IocRegistration.RegisterFeedServices(container);

            return container;
        }

        public RunnerBase(string testsToRun)
        {
            lock (_synclock)
            {
                if (_iocContainer == null)
                {
                    _iocContainer = InitializeIocContainer();
                }
            }
            _testsToRun = testsToRun;
        }

        protected bool ShouldRunTests(string testName)
        {
            if (string.Compare(_testsToRun, "all", true) == 0)
            {
                return true;
            }
            if (string.Compare(_testsToRun, testName, true) == 0)
            {
                return true;
            }
            return false;
        }

        protected void DisplayMessage(string message, DisplayLevel level = DisplayLevel.Message, Exception e = null)
        {
            lock (_synclock)
            {
                // keep all the message together
                if (e != null)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(e.ToString());
                    Console.ResetColor();
                }
                else
                {
                    switch (level)
                    {
                        case DisplayLevel.Error:
                            Console.ForegroundColor = ConsoleColor.Red;
                            break;
                        case DisplayLevel.Warning:
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            break;
                        case DisplayLevel.Message:
                            Console.ForegroundColor = ConsoleColor.White;
                            break;
                        case DisplayLevel.Title:
                            Console.ForegroundColor = ConsoleColor.Green;
                            break;
                        case DisplayLevel.Verbose:
                            Console.ForegroundColor = ConsoleColor.Gray;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException("level");
                    }
                    Console.WriteLine(message);
                    Console.ResetColor();
                }
            }
        }

        protected virtual void TestPreamble()
        {
            // nothing
        }

        protected virtual void TestPostamble()
        {
            // nothing
        }

        public void RunOneTest(Test theTest)
        {
            TestPreamble();
            theTest();
            TestPostamble();
        }
    }
}
