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
        protected readonly ReadOnlyControlFile _controlFile;

        public abstract void RunAllTests();

        private static LinFuIocContainer InitializeIocContainer()
        {
            var container = new LinFuIocContainer();

            IocRegistration.RegisterSystemServices(container);
            IocRegistration.RegisterFileServices(container);
            IocRegistration.RegisterFeedServices(container);

            return container;
        }

        public RunnerBase(string controlFilename)
        {
            lock (_synclock)
            {
                if (_iocContainer == null)
                {
                    _iocContainer = InitializeIocContainer();
                }
            }
            if (!string.IsNullOrEmpty(controlFilename))
            {
                _controlFile = new ReadOnlyControlFile(controlFilename);
                _verbose = _controlFile.GetDiagnosticOutput() == DiagnosticOutputLevel.Verbose;
            }
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
