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
using System.Reflection;
using System.IO;

using PodcastUtilities.Common;
using System.Xml;
using PodcastUtilities.Common.Configuration;
using PodcastUtilities.Common.Files;
using PodcastUtilities.Common.Platform;
using PodcastUtilities.Common.Playlists;
using PodcastUtilities.Ioc;

namespace GeneratePlaylist
{
    class Program
    {
        static private void DisplayBanner()
        {
            // do not move the GetExecutingAssembly call from here into a supporting DLL
            Assembly me = System.Reflection.Assembly.GetExecutingAssembly();
            AssemblyName name = me.GetName();
            Console.WriteLine("GeneratePlaylist v{0}", name.Version);
        }

        static private void DisplayHelp()
        {
            Console.WriteLine("Usage: GeneratePlaylist <controlfile>");
            Console.WriteLine("Where");
            Console.WriteLine("  <controlfile> = XML control file eg. podcasts.xml");
        }

        static void Main(string[] args)
        {
            DisplayBanner();
            if (args.Length < 1)
            {
                DisplayHelp();
                return;
            }

            IIocContainer iocContainer = InitializeIocContainer();

            var control = new ReadOnlyControlFile(args[0]);
            var finder = iocContainer.Resolve<IFinder>();
            var fileUtilities = iocContainer.Resolve<IFileUtilities>();
            var pathUtilities = iocContainer.Resolve<IPathUtilities>();
            var playlistFactory = iocContainer.Resolve<IPlaylistFactory>();

            var generator = new Generator(finder, fileUtilities, pathUtilities, playlistFactory);
            generator.StatusUpdate += new EventHandler<StatusUpdateEventArgs>(GeneratorStatusUpdate);

            if (!string.IsNullOrEmpty(control.GetPlaylistFileName()))
                generator.GeneratePlaylist(control, false);
        }

        private static IIocContainer InitializeIocContainer()
        {
            var container = IocRegistration.GetEmptyContainer();

            IocRegistration.RegisterFileServices(container);
            IocRegistration.RegisterPlaylistServices(container);

            return container;
        }

        static void GeneratorStatusUpdate(object sender, StatusUpdateEventArgs e)
        {
            // maybe we want to optionally filter verbose message
            Console.WriteLine(e.Message);
        }
    }
}
