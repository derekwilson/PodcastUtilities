using FakeItEasy;
using NUnit.Framework;
using PodcastUtilities.AndroidLogic.Utilities;
using PodcastUtilities.Common.Configuration;
using System;
using System.Xml;

namespace PodcastUtilities.AndroidTests.Tests.ViewModel.Configure
{
    [TestFixture]
    public class EditConfigViewModel_LoadControlFile : EditConfigViewModelBase
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
            Assert.AreEqual("Mocked control file error", ObservedResults.LastMessage);
        }
    }
}
