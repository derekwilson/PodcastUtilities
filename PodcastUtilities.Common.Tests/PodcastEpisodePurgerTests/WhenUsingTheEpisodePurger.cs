using System;
using System.Collections.Generic;
using System.IO;
using PodcastUtilities.Common.Platform;
using Rhino.Mocks;

namespace PodcastUtilities.Common.Tests.PodcastEpisodePurgerTests
{
    public abstract class WhenUsingTheEpisodePurger
        : WhenTestingBehaviour
    {
        protected PodcastEpisodePurger _episodePurger;

        protected IDirectoryInfoProvider _directoryInfoProvider;
        protected IFileUtilities _fileUtilities;
        protected ITimeProvider _timeProvider;

        protected string _rootFolder;
        protected PodcastInfo _podcastInfo;
        protected FeedInfo _feedInfo;
        protected IList<IFileInfo> _episodesToDelete;

        protected IDirectoryInfo _directoryInfo;
        protected IFileInfo[] _downloadedFiles;

        protected DateTime _now;
        private string _feedAddress;

        protected override void GivenThat()
        {
            base.GivenThat();

            _timeProvider = GenerateMock<ITimeProvider>();
            _fileUtilities = GenerateMock<IFileUtilities>();
            _directoryInfoProvider = GenerateMock<IDirectoryInfoProvider>();
            _directoryInfo = GenerateMock<IDirectoryInfo>();

            SetupData();
            SetupStubs();

            _episodePurger = new PodcastEpisodePurger(_fileUtilities, _timeProvider, _directoryInfoProvider);
        }

        protected virtual void SetupData()
        {
            _now = new DateTime(2010,5,1,16,10,12);

            _feedAddress = "http://test";

            _feedInfo = new FeedInfo();
            _feedInfo.Format = PodcastFeedFormat.RSS;
            _feedInfo.NamingStyle = PodcastEpisodeNamingStyle.UrlFilename;
            _feedInfo.Address = new Uri(_feedAddress);
            _feedInfo.MaximumDaysOld = int.MaxValue;
            _feedInfo.DownloadStrategy = PodcastEpisodeDownloadStrategy.All;
            _feedInfo.DeleteDownloadsDaysOld = int.MaxValue;

            _rootFolder = "c:\\TestRoot";
            _podcastInfo = new PodcastInfo();
            _podcastInfo.Folder = "TestFolder";
            _podcastInfo.Feed = _feedInfo;
        }

        protected virtual void SetupStubs()
        {
            _timeProvider.Stub(time => time.UtcNow).Return(_now);
            _directoryInfo.Stub(dir => dir.GetFiles("*.*")).Return(_downloadedFiles);
            _directoryInfoProvider.Stub(prov => prov.GetDirectoryInfo(Path.Combine(_rootFolder, _podcastInfo.Folder))).Return(_directoryInfo);
        }

        protected virtual void StubFiles()
        {
            _downloadedFiles = new IFileInfo[]
            {
			    GenerateMock<IFileInfo>(),
				GenerateMock<IFileInfo>(),
				GenerateMock<IFileInfo>(),
				GenerateMock<IFileInfo>(),
				GenerateMock<IFileInfo>()
			};

            _downloadedFiles[0].Stub(file => file.CreationTime).Return(new DateTime(2010, 4, 30, 16, 11, 12));
            _downloadedFiles[1].Stub(file => file.CreationTime).Return(new DateTime(2010, 4, 26, 16, 11, 12));
            _downloadedFiles[2].Stub(file => file.CreationTime).Return(new DateTime(2010, 4, 26, 16, 09, 12));
            _downloadedFiles[3].Stub(file => file.CreationTime).Return(new DateTime(2010, 4, 20, 16, 11, 12));
            _downloadedFiles[4].Stub(file => file.CreationTime).Return(new DateTime(2000, 4, 20, 16, 11, 12));

            _downloadedFiles[0].Stub(file => file.FullName).Return(Path.Combine(Path.Combine(_rootFolder, _podcastInfo.Folder), "2010_04_30_1611_title_.mp3"));
            _downloadedFiles[1].Stub(file => file.FullName).Return(Path.Combine(Path.Combine(_rootFolder, _podcastInfo.Folder), "2010_04_26_1611_title_.mp3"));
            _downloadedFiles[2].Stub(file => file.FullName).Return(Path.Combine(Path.Combine(_rootFolder, _podcastInfo.Folder), "2010_04_26_1609_title_.mp3"));
            _downloadedFiles[3].Stub(file => file.FullName).Return(Path.Combine(Path.Combine(_rootFolder, _podcastInfo.Folder), "2010_04_20_1611_title_.mp3"));
            _downloadedFiles[4].Stub(file => file.FullName).Return(Path.Combine(Path.Combine(_rootFolder, _podcastInfo.Folder), "state.xml"));
        }
}
}
