using PodcastUtilities.Common;
using PodcastUtilities.Common.Configuration;
using PodcastUtilities.Common.Feeds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PodcastUtilities.Integration.Tests.Download
{
	class Runner : RunnerBase
	{
#if NETFULL
		private const string _inputfilename = "test.windows.controlfile.xml";
#else
		private const string _inputfilename = "test.standard.controlfile.xml";
#endif


		public Runner(string testToRun)
			: base(testToRun)
		{
		}

		public override void RunAllTests()
		{
			DisplayMessage("Download Tests:", DisplayLevel.Title);
			if (!ShouldRunTests("download"))
			{
				DisplayMessage(" tests skipped");
				return;
			}

			RunOneTest(ReadFeed1);
			RunOneTest(DownloadFile1);
		}

		private PodcastInfo GetPodcastInfo(ReadOnlyControlFile controlFile, int index)
		{
			IEnumerable<PodcastInfo> podcasts = controlFile.GetPodcasts();
			PodcastInfo info = podcasts.ElementAt(index);
			return info;
		}

		private IList<ISyncItem> GetAllEpisodesInFeed(ReadOnlyControlFile controlFile, PodcastInfo info)
		{
			List<ISyncItem> allEpisodes = new List<ISyncItem>(20);
			IEpisodeFinder podcastEpisodeFinder = _iocContainer.Resolve<IEpisodeFinder>();
			IList<ISyncItem> episodesInThisFeed = podcastEpisodeFinder.FindEpisodesToDownload(
					controlFile.GetSourceRoot(),
					controlFile.GetRetryWaitInSeconds(),
					info,
					controlFile.GetDiagnosticRetainTemporaryFiles()
			);
			allEpisodes.AddRange(episodesInThisFeed);
			return allEpisodes;
		}

		private void ReadFeed1()
		{
			ReadOnlyControlFile controlFile = new ReadOnlyControlFile(_inputfilename);
			PodcastInfo info = GetPodcastInfo(controlFile, 0);
			DisplayMessage(string.Format("Reading a feed: {0}", info.Feed.Address));

			IList<ISyncItem> allEpisodes = GetAllEpisodesInFeed(controlFile, info);
			DisplayMessage(string.Format("Eposodes in feed: {0}", allEpisodes.Count));
			foreach (ISyncItem item in allEpisodes)
			{
				DisplayMessage(string.Format("Eposode: {0}", item.EpisodeTitle));
			}
		}


		private void DownloadFile1()
		{
			ReadOnlyControlFile controlFile = new ReadOnlyControlFile(_inputfilename);
			PodcastInfo info = GetPodcastInfo(controlFile, 0);
			DisplayMessage(string.Format("Reading a feed: {0}", info.Feed.Address));
			IList<ISyncItem> allEpisodes = GetAllEpisodesInFeed(controlFile, info);
			if (allEpisodes.Count < 1)
			{
				DisplayMessage("No episodes in the feed - dont forget the state.xml file is being used", DisplayLevel.Warning);
				return;
			}
			IList<ISyncItem> firstEpisode = new List<ISyncItem>(1);
			firstEpisode.Add(allEpisodes.First());

			DisplayMessage(string.Format("Downloading Eposode: {0}", firstEpisode.First().EpisodeTitle));
			ISyncItemToEpisodeDownloaderTaskConverter converter = _iocContainer.Resolve<ISyncItemToEpisodeDownloaderTaskConverter>();
			IEpisodeDownloader[] downloadTasks = converter.ConvertItemsToTasks(firstEpisode, StatusUpdate, ProgressUpdate);

			// run them in a task pool
			ITaskPool taskPool = _iocContainer.Resolve<ITaskPool>();
			taskPool.RunAllTasks(1, downloadTasks);

			DisplayMessage(string.Format("Download Complete", allEpisodes.Count));
		}
	}
}
