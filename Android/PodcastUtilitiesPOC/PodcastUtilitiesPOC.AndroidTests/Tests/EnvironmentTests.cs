using Android.Content.Res;
using PodcastUtilities.Common.Platform;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Xunit;
using Xunit.Abstractions;

namespace PodcastUtilitiesPOC.AndroidTests.Tests
{
    public class EnvironmentTests
    {
        ITestOutputHelper Output;

        public EnvironmentTests(ITestOutputHelper output)
        {
            Output = output;
        }

        [Fact]
        public void Assertion_Succeeds()
        {
            Output.WriteLine($"Step 10");

            var writeTime = File.GetLastWriteTime(Assembly.GetExecutingAssembly().Location);
            Output.WriteLine($"Write Time (Local): {writeTime.ToLocalTime().ToString()}");

            List<string> environment = WindowsEnvironmentInformationProvider.GetEnvironmentRuntimeDisplayInformation();
            foreach (var line in environment)
            {
                Output.WriteLine(line);
            }
            Assert.Equal<int>(4, 2+2);
        }

    }
}