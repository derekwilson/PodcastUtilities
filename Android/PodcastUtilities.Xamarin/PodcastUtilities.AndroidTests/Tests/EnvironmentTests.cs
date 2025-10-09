using NUnit.Framework;
using PodcastUtilities.Common.Platform;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace PodcastUtilities.AndroidTests.Tests
{
    [TestFixture]
    public class EnvironmentTests
    {
        [SetUp]
        public void Setup()
        {
            // write your test fixture setup
        }

        [TearDown]
        public void TearDown()
        {
            // write your test fixture teardown 
        }

        [Test]
        public void Assertion_Succeeds()
        {
            var writeTime = File.GetLastWriteTime(Assembly.GetExecutingAssembly().Location);
            Console.WriteLine($"Write Time (Local): {writeTime.ToLocalTime().ToString()}");
            List<string> environment = WindowsEnvironmentInformationProvider.GetEnvironmentRuntimeDisplayInformation();
            foreach (var line in environment)
            {
                Console.WriteLine(line);
            }
            Assert.AreEqual(4, 2 + 2);
        }
    }

}