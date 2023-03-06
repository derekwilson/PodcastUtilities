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
using System.IO;
using Moq;
using PodcastUtilities.Common.Configuration;
using PodcastUtilities.Common.Files;
using PodcastUtilities.Common.Platform;

namespace PodcastUtilities.Common.Multiplatform.Tests.Files.EpisodePurgerTests
{
    public abstract class WhenUsingTheEpisodePurger
        : WhenTestingBehaviour
    {
        protected EpisodePurger _episodePurger;

        protected Mock<IDirectoryInfoProvider> _directoryInfoProvider;
        protected Mock<ITimeProvider> _timeProvider;
        protected Mock<IFileUtilities> _fileUtilities;

        protected string _rootFolder;
        protected PodcastInfo _podcastInfo;
        protected FeedInfo _feedInfo;
        protected IList<IFileInfo> _episodesToDelete;
        protected IList<IDirectoryInfo> _foldersToDelete;

        protected Mock<IDirectoryInfo> _directoryInfo;
        protected IFileInfo[] _downloadedFiles;
        protected IFileInfo[] _allFiles;
        protected IDirectoryInfo[] _subFolders;
        protected Mock<IDirectoryInfo> _folder1;
        protected Mock<IDirectoryInfo> _folder2;
        protected Mock<IFileInfo> _file1;
        protected Mock<IFileInfo> _file2;
        protected Mock<IFileInfo> _file3;
        protected Mock<IFileInfo> _file4;
        protected Mock<IFileInfo> _file5;
        protected Mock<IFileInfo> _file6;

        protected DateTime _now;
        private string _feedAddress;
        protected IReadOnlyControlFile _controlFile;

        protected override void GivenThat()
        {
            base.GivenThat();

            _controlFile = TestControlFileFactory.CreateControlFile();

            _timeProvider = GenerateMock<ITimeProvider>();
            _directoryInfoProvider = GenerateMock<IDirectoryInfoProvider>();
            _directoryInfo = GenerateMock<IDirectoryInfo>();
            _fileUtilities = GenerateMock<IFileUtilities>();

            SetupData();
            SetupStubs();

            _episodePurger = new EpisodePurger(_timeProvider.Object, _directoryInfoProvider.Object, _fileUtilities.Object);
        }

        protected virtual void SetupData()
        {
            _now = new DateTime(2010, 5, 1, 16, 10, 12);

            _feedAddress = "http://test";

            _feedInfo = new FeedInfo(_controlFile);
            _feedInfo.Format.Value = PodcastFeedFormat.RSS;
            _feedInfo.NamingStyle.Value = PodcastEpisodeNamingStyle.UrlFileName;
            _feedInfo.Address = new Uri(_feedAddress);
            _feedInfo.MaximumDaysOld.Value = int.MaxValue;
            _feedInfo.DownloadStrategy.Value = PodcastEpisodeDownloadStrategy.All;
            _feedInfo.DeleteDownloadsDaysOld.Value = int.MaxValue;

            _rootFolder = "c:\\TestRoot";
            _podcastInfo = new PodcastInfo(_controlFile);
            _podcastInfo.Folder = "TestFolder";
            _podcastInfo.Pattern.Value = "*.mp3";
            _podcastInfo.DeleteEmptyFolder.Value = true;
            _podcastInfo.Feed = _feedInfo;
        }

        protected virtual void SetupStubs()
        {
            _timeProvider.Setup(time => time.UtcNow).Returns(_now);
            _directoryInfo.Setup(dir => dir.GetFiles(_podcastInfo.Pattern.Value)).Returns(_downloadedFiles);
            _directoryInfo.Setup(dir => dir.GetFiles("*.*")).Returns(_allFiles);
            _directoryInfo.Setup(dir => dir.GetDirectories("*.*")).Returns(_subFolders);
            _directoryInfoProvider.Setup(prov => prov.GetDirectoryInfo(Path.Combine(_rootFolder, _podcastInfo.Folder))).Returns(_directoryInfo.Object);

            // yes I know I return the same folder twice but it dows not need to be a real file system
            _directoryInfoProvider.Setup(prov => prov.GetDirectoryInfo(Path.Combine(Path.Combine(_rootFolder, _podcastInfo.Folder), "sub1"))).Returns(_directoryInfo.Object);
            _directoryInfoProvider.Setup(prov => prov.GetDirectoryInfo(Path.Combine(Path.Combine(_rootFolder, _podcastInfo.Folder), "sub2"))).Returns(_directoryInfo.Object);
        }

        protected virtual void StubSubFolders()
        {
            _folder1 = GenerateMock<IDirectoryInfo>();
            _folder2 = GenerateMock<IDirectoryInfo>();
            _subFolders = new IDirectoryInfo[]
            {
                _folder1.Object,
                _folder2.Object,
            };

            _folder1.Setup(dir => dir.GetFiles(_podcastInfo.Pattern.Value)).Returns(_downloadedFiles);
            _folder2.Setup(dir => dir.GetFiles(_podcastInfo.Pattern.Value)).Returns(_downloadedFiles);

            _folder1.Setup(dir => dir.FullName).Returns(Path.Combine(Path.Combine(_rootFolder, _podcastInfo.Folder), "sub1"));
            _folder2.Setup(dir => dir.FullName).Returns(Path.Combine(Path.Combine(_rootFolder, _podcastInfo.Folder), "sub2"));
        }

        protected virtual void StubFiles()
        {
            _file1 = GenerateMock<IFileInfo>();
            _file2 = GenerateMock<IFileInfo>();
            _file3 = GenerateMock<IFileInfo>();
            _file4 = GenerateMock<IFileInfo>();
            _file5 = GenerateMock<IFileInfo>();
            _file6 = GenerateMock<IFileInfo>();
            _downloadedFiles = new IFileInfo[]
            {
                _file1.Object,
                _file2.Object,
                _file3.Object,
                _file4.Object,
            };

            _allFiles = new IFileInfo[]
            {
                _file1.Object,
                _file2.Object,
                _file3.Object,
                _file4.Object,
                _file5.Object,
                _file6.Object,
            };

            _file1.Setup(file => file.CreationTime).Returns(new DateTime(2010, 4, 30, 16, 11, 12));
            _file2.Setup(file => file.CreationTime).Returns(new DateTime(2010, 4, 26, 16, 11, 12));
            _file3.Setup(file => file.CreationTime).Returns(new DateTime(2010, 4, 26, 16, 09, 12));
            _file4.Setup(file => file.CreationTime).Returns(new DateTime(2010, 4, 20, 16, 11, 12));
            _file5.Setup(file => file.CreationTime).Returns(new DateTime(2000, 4, 20, 16, 11, 12));
            _file6.Setup(file => file.CreationTime).Returns(new DateTime(2000, 4, 20, 16, 11, 12));

            _file1.Setup(file => file.FullName).Returns(Path.Combine(Path.Combine(_rootFolder, _podcastInfo.Folder), "2010_04_30_1611_title_.mp3"));
            _file2.Setup(file => file.FullName).Returns(Path.Combine(Path.Combine(_rootFolder, _podcastInfo.Folder), "2010_04_26_1611_title_.mp3"));
            _file3.Setup(file => file.FullName).Returns(Path.Combine(Path.Combine(_rootFolder, _podcastInfo.Folder), "2010_04_26_1609_title_.mp3"));
            _file4.Setup(file => file.FullName).Returns(Path.Combine(Path.Combine(_rootFolder, _podcastInfo.Folder), "2010_04_20_1611_title_.mp3"));
            _file5.Setup(file => file.FullName).Returns(Path.Combine(Path.Combine(_rootFolder, _podcastInfo.Folder), "thumbs.db"));
            _file6.Setup(file => file.FullName).Returns(Path.Combine(Path.Combine(_rootFolder, _podcastInfo.Folder), "state.xml"));

            _file1.Setup(file => file.Name).Returns("2010_04_30_1611_title_.mp3");
            _file2.Setup(file => file.Name).Returns("2010_04_26_1611_title_.mp3");
            _file3.Setup(file => file.Name).Returns("2010_04_26_1609_title_.mp3");
            _file4.Setup(file => file.Name).Returns("2010_04_20_1611_title_.mp3");
            _file5.Setup(file => file.Name).Returns("thumbs.db");
            _file6.Setup(file => file.Name).Returns("state.xml");
        }
    }
}
