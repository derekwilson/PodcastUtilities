using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using PodcastUtilities.Common.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            Output.WriteLine($"Running environment test on ");
            List<string> environment = WindowsEnvironmentInformationProvider.GetEnvironmentRuntimeDisplayInformation();
            foreach (var line in environment)
            {
                Output.WriteLine(line);
            }
            Assert.Equal<int>(4, 2+2);
        }

    }
}