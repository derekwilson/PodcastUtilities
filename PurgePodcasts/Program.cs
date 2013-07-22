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
using System.Reflection;
using System.Text;
using PodcastUtilities.Common;
using PodcastUtilities.Common.Configuration;
using PodcastUtilities.Common.Files;
using PodcastUtilities.Common.Platform;
using PodcastUtilities.Ioc;

namespace PurgePodcasts
{
    class Program
    {
        static LinFuIocContainer _iocContainer;
        static ReadOnlyControlFile _control;
        private static bool _quiet = false;

        static private void DisplayBanner()
        {
            // do not move the GetExecutingAssembly call from here into a supporting DLL
            Assembly me = System.Reflection.Assembly.GetExecutingAssembly();
            AssemblyName name = me.GetName();
            Console.WriteLine("PurgePodcasts v{0}", name.Version);
        }

        static private void DisplayHelp()
        {
            Console.WriteLine("Usage: PurgePodcasts <controlfile>");
            Console.WriteLine("Where");
            Console.WriteLine("  <controlfile> = XML control file eg. podcasts.xml");
        }

        private static LinFuIocContainer InitializeIocContainer()
        {
            var container = new LinFuIocContainer();

            IocRegistration.RegisterPortableDeviceServices(container);
            IocRegistration.RegisterSystemServices(container);
            IocRegistration.RegisterFileServices(container);

            return container;
        }

        static void Main(string[] args)
        {
            DisplayBanner();
            if (args.Length < 1)
            {
                DisplayHelp();
                return;
            }

            _iocContainer = InitializeIocContainer();
            _control = new ReadOnlyControlFile(args[0]);
            if (args.Count() > 1)
            {
                _quiet = args[1].Contains('q');
            }

            // find the episodes to delete
            var allFilesToDelete = new List<IFileInfo>(20);
            var podcastEpisodePurger = _iocContainer.Resolve<IEpisodePurger>();
            foreach (var podcastInfo in _control.GetPodcasts())
            {
                var filesToDeleteFromThisFeed = podcastEpisodePurger.FindEpisodesToPurge(_control.GetSourceRoot(), podcastInfo);
                allFilesToDelete.AddRange(filesToDeleteFromThisFeed);
            }

            if (allFilesToDelete.Count == 0)
            {
                Console.WriteLine("There are no files to delete");
                return;
            }

            var fileUtilities = _iocContainer.Resolve<IFileUtilities>();
            if (_quiet)
            {
                foreach (var fileInfo in allFilesToDelete)
                {
                    Console.WriteLine("Deleted: {0}", fileInfo.FullName);
                    fileUtilities.FileDelete(fileInfo.FullName);
                }
            }
            else
            {
                foreach (var fileInfo in allFilesToDelete)
                {
                    Console.WriteLine("{0}", fileInfo.FullName);
                }
                Console.WriteLine("OK to delete ALL the above files? (y/n) ");
                string answer;
                do
                {
                    char key = Convert.ToChar(Console.Read());
                    answer = key.ToString().ToLower();
                } while (answer != "y" && answer != "n");
                if (answer == "y")
                {
                    Console.WriteLine("Deleting {0} files",allFilesToDelete.Count);
                    foreach (var fileInfo in allFilesToDelete)
                    {
                        fileUtilities.FileDelete(fileInfo.FullName);
                    }
                }
                else
                {
                    Console.WriteLine("No files deleted");
                }
            }

            Console.WriteLine("Done");
        }
    }
}
