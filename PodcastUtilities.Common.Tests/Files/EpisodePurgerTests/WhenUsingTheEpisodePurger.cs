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
using PodcastUtilities.Common.Configuration;
using PodcastUtilities.Common.Files;
using PodcastUtilities.Common.Platform;
using Rhino.Mocks;

namespace PodcastUtilities.Common.Tests.Files.EpisodePurgerTests
{
    public abstract class WhenUsingTheEpisodePurger
        : WhenTestingBehaviour
    {
        protected EpisodePurger _episodePurger;

        protected IDirectoryInfoProvider _directoryInfoProvider;
        protected ITimeProvider _timeProvider;
        protected IFileUtilities _fileUtilities;

        protected string _rootFolder;
        protected PodcastInfo _podcastInfo;
        protected FeedInfo _feedInfo;
        protected IList<IFileInfo> _episodesToDelete;
        protected IList<IDirectoryInfo> _foldersToDelete; 

        protected IDirectoryInfo _directoryInfo;
        protected IFileInfo[] _downloadedFiles;
        protected IDirectoryInfo[] _subFolders;

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

            _episodePurger = new EpisodePurger(_timeProvider, _directoryInfoProvider, _fileUtilities);
        }

        protected virtual void SetupData()
        {
            _now = new DateTime(2010,5,1,16,10,12);

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
            _timeProvider.Stub(time => time.UtcNow).Return(_now);
            _directoryInfo.Stub(dir => dir.GetFiles(_podcastInfo.Pattern.Value)).Return(_downloadedFiles);
            _directoryInfo.Stub(dir => dir.GetFiles("*.*")).Return(_downloadedFiles);
            _directoryInfo.Stub(dir => dir.GetDirectories("*.*")).Return(_subFolders);
            _directoryInfoProvider.Stub(prov => prov.GetDirectoryInfo(Path.Combine(_rootFolder, _podcastInfo.Folder))).Return(_directoryInfo);

            // yes I know I return the same folder twice but it dows not need to be a real file system
            _directoryInfoProvider.Stub(prov => prov.GetDirectoryInfo(Path.Combine(Path.Combine(_rootFolder, _podcastInfo.Folder), "sub1"))).Return(_directoryInfo);
            _directoryInfoProvider.Stub(prov => prov.GetDirectoryInfo(Path.Combine(Path.Combine(_rootFolder, _podcastInfo.Folder), "sub2"))).Return(_directoryInfo);
        }

        protected virtual void StubSubFolders()
        {
            _subFolders = new IDirectoryInfo[]
            {
			    GenerateMock<IDirectoryInfo>(),
				GenerateMock<IDirectoryInfo>(),
			};

            _subFolders[0].Stub(dir => dir.GetFiles(_podcastInfo.Pattern.Value)).Return(_downloadedFiles);
            _subFolders[1].Stub(dir => dir.GetFiles(_podcastInfo.Pattern.Value)).Return(_downloadedFiles);

            _subFolders[0].Stub(dir => dir.FullName).Return(Path.Combine(Path.Combine(_rootFolder, _podcastInfo.Folder), "sub1"));
            _subFolders[1].Stub(dir => dir.FullName).Return(Path.Combine(Path.Combine(_rootFolder, _podcastInfo.Folder), "sub2"));
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

            _downloadedFiles[0].Stub(file => file.Name).Return("2010_04_30_1611_title_.mp3");
            _downloadedFiles[1].Stub(file => file.Name).Return("2010_04_26_1611_title_.mp3");
            _downloadedFiles[2].Stub(file => file.Name).Return("2010_04_26_1609_title_.mp3");
            _downloadedFiles[3].Stub(file => file.Name).Return("2010_04_20_1611_title_.mp3");
            _downloadedFiles[4].Stub(file => file.Name).Return("state.xml");
        }
    }
}
