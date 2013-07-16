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
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using PodcastUtilities.Common.Perfmon;
using PodcastUtilities.Common.Platform;
using PodcastUtilities.Ioc;

namespace PerfmonCountersInstaller
{
    class Program
    {
        static LinFuIocContainer _iocContainer;

        static private void DisplayBanner()
        {
            // do not move the GetExecutingAssembly call from here into a supporting DLL
            Assembly me = System.Reflection.Assembly.GetExecutingAssembly();
            AssemblyName name = me.GetName();
            Console.WriteLine("PerfmonCountersInstaller v{0}", name.Version);
        }

        static private void DisplayHelp()
        {
            Console.WriteLine("Usage: PerfmonCountersInstaller <option>");
            Console.WriteLine("Where");
            Console.WriteLine("  <option> = del to delete the counters and nothing to install them");
        }

        private static LinFuIocContainer InitializeIocContainer()
        {
            var container = new LinFuIocContainer();

            IocRegistration.RegisterSystemServices(container);

            return container;
        }

        private static void TestCounters()
        {
            var factory = _iocContainer.Resolve<ICounterFactory>();

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            Thread.Sleep(1000);
            
            var aveCounter1 = factory.CreateAverageCounter(CategoryInstaller.PodcastUtilitiesCommonCounterCategory,
                                                 CategoryInstaller.AverageTimeToDownload,
                                                 CategoryInstaller.NumberOfDownloads);
            var aveCounter2 = factory.CreateAverageCounter(CategoryInstaller.PodcastUtilitiesCommonCounterCategory,
                                                 CategoryInstaller.AverageMBDownload,
                                                 CategoryInstaller.SizeOfDownloads);
            aveCounter1.RegisterTime(stopwatch);
            aveCounter2.RegisterValue(5);
        }

        static void Main(string[] args)
        {
            DisplayBanner();
            if (args.Length > 0 && args[0].ToUpperInvariant() != "DEL" && args[0].ToUpperInvariant() != "TEST")
            {
                DisplayHelp();
                return;
            }

            _iocContainer = InitializeIocContainer();

            var installer = _iocContainer.Resolve<ICategoryInstaller>();
            CategoryInstallerRefeshResult result = CategoryInstallerRefeshResult.Unknown;

            if (args.Length > 0 && args[0].ToUpperInvariant() == "DEL")
            {
                result = installer.DeleteCatagory(CategoryInstaller.PodcastUtilitiesCommonCounterCategory);
            }
            else if (args.Length > 0 && args[0].ToUpperInvariant() == "TEST")
            {
                TestCounters();
                return;
            }
            else
            {
                installer.AddCounter(CategoryInstaller.AverageTimeToDownload, "Measures ms for the download call",PerformanceCounterType.AverageCount64);
                installer.AddCounter(CategoryInstaller.AverageTimeToDownload + "Base", "Measures ms for the download call",PerformanceCounterType.AverageBase);
                installer.AddCounter(CategoryInstaller.NumberOfDownloads, "Total number of downloads", PerformanceCounterType.NumberOfItems64);
                installer.AddCounter(CategoryInstaller.AverageMBDownload, "Measures MB for the download call", PerformanceCounterType.AverageCount64);
                installer.AddCounter(CategoryInstaller.AverageMBDownload + "Base", "Measures MB for the download call", PerformanceCounterType.AverageBase);
                installer.AddCounter(CategoryInstaller.SizeOfDownloads, "Total size of downloads in kb", PerformanceCounterType.NumberOfItems64);

                result = installer.RefreshCatagoryWithCounters(CategoryInstaller.PodcastUtilitiesCommonCounterCategory,"PodcastUtilities.Common counters");
            }

            switch (result)
            {
                case CategoryInstallerRefeshResult.CatagoryCreated:
                    Console.WriteLine("{0} catagory created", CategoryInstaller.PodcastUtilitiesCommonCounterCategory);
                    break;
                case CategoryInstallerRefeshResult.CatagoryUpdated:
                    Console.WriteLine("{0} catagory updated", CategoryInstaller.PodcastUtilitiesCommonCounterCategory);
                    break;
                case CategoryInstallerRefeshResult.CatagoryDeleted:
                    Console.WriteLine("{0} catagory deleted", CategoryInstaller.PodcastUtilitiesCommonCounterCategory);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
