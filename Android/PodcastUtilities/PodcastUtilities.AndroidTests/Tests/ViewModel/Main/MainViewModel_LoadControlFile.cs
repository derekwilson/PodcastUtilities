using Android.Net;
using FakeItEasy;
using NUnit.Framework;
using PodcastUtilities.AndroidLogic.Utilities;
using PodcastUtilities.Common.Configuration;
using System;
using System.Xml;

namespace PodcastUtilities.AndroidTests.Tests.ViewModel.Main
{
    [TestFixture]
    public class MainViewModel_LoadControlFile : MainViewModelBase
    {
        private IReadWriteControlFile SetupRealControlFile(Android.Net.Uri uri)
        {
            FileSystemHelper helper = new FileSystemHelper(MainActivity.MainContext, MockLogger);
            XmlDocument xml = helper.LoadXmlFromAssetFile("xml/testcontrolfile1.xml");
            A.CallTo(() => MockFileSystemHelper.LoadXmlFromContentUri(uri)).Returns(xml);
            IReadWriteControlFile control = new ReadWriteControlFile(xml);
            A.CallTo(() => MockApplicationControlFileFactory.CreateControlFile(xml)).Returns(control);
            return control;
        }

        [Test]
        public void LoadControlFile_LoadsFile()
        {
            // arrange
            Android.Net.Uri uri = Android.Net.Uri.Parse("content://com.android.externalstorage.documents/document/primary%3APodcastUtilities%2FDerekPodcastsSmall2.xml");
            var control = SetupRealControlFile(uri);
            ViewModel.Initialise();

            // act
            ViewModel.LoadContolFile(uri);

            // assert
            A.CallTo(() => MockCrashReporter.LogNonFatalException(A<Exception>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => MockApplicationControlFileProvider.ReplaceApplicationConfiguration(control)).MustHaveHappened(1, Times.Exactly);
        }

        [Test]
        public void LoadControlFile_RecordsAnalytics()
        {
            // arrange
            Android.Net.Uri uri = Android.Net.Uri.Parse("content://com.android.externalstorage.documents/document/primary%3APodcastUtilities%2FDerekPodcastsSmall2.xml");
            var control = SetupRealControlFile(uri);
            ViewModel.Initialise();

            // act
            ViewModel.LoadContolFile(uri);

            // assert
            A.CallTo(() => MockAnalyticsEngine.LoadControlFileEvent()).MustHaveHappened(1, Times.Exactly);
        }

        [Test]
        public void LoadControlFile_RefreshesFeeds()
        {
            // arrange
            Android.Net.Uri uri = Android.Net.Uri.Parse("content://com.android.externalstorage.documents/document/primary%3APodcastUtilities%2FDerekPodcastsSmall2.xml");
            var control = SetupRealControlFile(uri);
            ViewModel.Initialise();
            A.CallTo(() => MockApplicationControlFileProvider.GetApplicationConfiguration()).Returns(control);

            // act
            ViewModel.LoadContolFile(uri);

            // assert
            Assert.AreEqual("/sdcard/Podcasts", ObservedResults.LastSetCacheRoot);
            Assert.AreEqual("feed count == 4", ObservedResults.LastSetFeedHeading);
            Assert.AreEqual(4, ObservedResults.LastSetFeedItems.Count);
            Assert.AreEqual("Cricinfo The Switch Hit Cricket Show", ObservedResults.LastSetFeedItems[0].PodcastFeed.Folder);
            Assert.AreEqual("Friday Night Comedy from BBC Radio 4", ObservedResults.LastSetFeedItems[1].PodcastFeed.Folder);
            Assert.AreEqual("From Our Own Correspondent", ObservedResults.LastSetFeedItems[2].PodcastFeed.Folder);
            Assert.AreEqual("Mark Kermode and Simon Mayos Film Reviews", ObservedResults.LastSetFeedItems[3].PodcastFeed.Folder);

            Assert.AreEqual("Mocked Control file loaded", ObservedResults.LastToastMessage);
        }

        [Test]
        public void LoadControlFile_HandlesErrors()
        {
            // arrange
            Android.Net.Uri uri = Android.Net.Uri.Parse("content://com.android.externalstorage.documents/document/primary%3APodcastUtilities%2FDerekPodcastsSmall2.xml");
            var control = SetupRealControlFile(uri);
            ViewModel.Initialise();
            var testException = new Exception();
            A.CallTo(() => MockApplicationControlFileFactory.CreateControlFile(A<XmlDocument>.Ignored)).Throws(testException);

            // act
            ViewModel.LoadContolFile(uri);

            // assert
            A.CallTo(() => MockCrashReporter.LogNonFatalException(testException)).MustHaveHappened(1, Times.Exactly);
            Assert.AreEqual("Mocked control file error", ObservedResults.LastToastMessage);
        }
    }
}