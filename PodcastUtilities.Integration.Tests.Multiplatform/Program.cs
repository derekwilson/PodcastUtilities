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
using PodcastUtilities.Common.Platform;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace PodcastUtilities.Integration.Tests
{
    class Program
    {
        static string GetOSDescription()
        {
            // its a bit pants but it has taken a long time for System.Runtime.InteropServices to be available in NET FULL
#if NETFULL
            return ".NET Framework " + Environment.OSVersion;
#else
            return System.Runtime.InteropServices.RuntimeInformation.OSDescription;
#endif
        }

        static private void DisplayBanner()
        {
            // do not move the GetExecutingAssembly call from here into a supporting DLL
            Assembly me = System.Reflection.Assembly.GetExecutingAssembly();
            AssemblyName name = me.GetName();
            Console.WriteLine("PodcastUtilities.Integration.Tests v{0}", name.Version);
            Console.WriteLine($"OS = {GetOSDescription()}");
        }

        static private void DisplayHelp()
        {
            Console.Write("Running on ");
            List<string> envirnment = WindowsEnvironmentInformationProvider.GetEnvironmentRuntimeDisplayInformation();
            foreach (string line in envirnment)
            {
                Console.WriteLine(line);
            }
            Console.WriteLine("Usage: PodcastUtilities.Integration.Tests <tests>");
            Console.WriteLine("Where");
            Console.WriteLine("  <tests> = all: run all tests");
#if NETFULL
            Console.WriteLine("          = mtp: run portable device tests");
#endif
            Console.WriteLine("          = control: run control file tests");
            Console.WriteLine("          = download: run download tests");
        }

        static void Main(string[] args)
        {
            DisplayBanner();
            string testToRun = null;
            if (args.Length < 1)
            {
                DisplayHelp();
                return;
            }
            else
            {
                testToRun = args[0];
            }

            var controlFileTests = new ControlFile.Runner(testToRun);
            controlFileTests.RunAllTests();

            var downloadTests = new Download.Runner(testToRun);
            downloadTests.RunAllTests();

#if NETFULL
            var portableDevicesTests = new PortableDevices.Runner(testToRun);
            portableDevicesTests.RunAllTests();
#endif

            Console.WriteLine("Done");
        }
    }
}
