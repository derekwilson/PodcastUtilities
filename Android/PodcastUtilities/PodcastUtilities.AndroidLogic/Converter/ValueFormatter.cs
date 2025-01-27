﻿using PodcastUtilities.AndroidLogic.Logging;
using PodcastUtilities.AndroidLogic.Utilities;
using PodcastUtilities.Common.Configuration;
using PodcastUtilities.Common.Platform;
using PodcastUtilities.Common.Playlists;
using System.Collections.Generic;
using System.Globalization;

namespace PodcastUtilities.AndroidLogic.Converter
{
    public interface IValueFormatter
    {
        string GetNamingStyleText(PodcastEpisodeNamingStyle namingstyle);
        string GetNamingStyleTextLong(PodcastEpisodeNamingStyle namingstyle);
        string GetDownloadStratagyText(PodcastEpisodeDownloadStrategy strategy);
        string GetDownloadStratagyTextLong(PodcastEpisodeDownloadStrategy strategy);
        string GetCustomOrNamedIntValue(int namedTextId, int namedValue, int customFormatId, int value);
        string GetDefaultableCustomOrNamedIntValue(int namedTextId, int namedValue, int customFormatId, IDefaultableItem<int> item);
        string GetFeedOverrideSummary(IPodcastInfo podcastInfo);
        string GetDefaultableNamingStyleTextLong(IDefaultableItem<PodcastEpisodeNamingStyle> namingStyle);
        string GetDefaultableDownloadStratagyTextLong(IDefaultableItem<PodcastEpisodeDownloadStrategy> downloadStrategy);
        string GetPlaylistFormatTextLong(PlaylistFormat playlistFormat);
        string GetDiagOutputTextLong(DiagnosticOutputLevel diagnosticOutputLevel);
        string GetDayOffsetAsDateString(int offset);
    }

    public class ValueFormatter : IValueFormatter
    {
        private ICrashReporter CrashReporter;
        private ILogger Logger;
        private IResourceProvider ResourceProvider;
        private IFileSystemHelper FileSystemHelper;
        private ITimeProvider TimeProvider;

        public ValueFormatter(
            ICrashReporter crashReporter,
            ILogger logger,
            IResourceProvider resourceProvider,
            IFileSystemHelper fileSystemHelper,
            ITimeProvider timeProvider)
        {
            CrashReporter = crashReporter;
            Logger = logger;
            ResourceProvider = resourceProvider;
            FileSystemHelper = fileSystemHelper;
            TimeProvider = timeProvider;
        }

        private const string SEPERATOR = " - ";

        private int GetNamingStyleTextId(PodcastEpisodeNamingStyle namingStyle)
        {
            switch (namingStyle)
            {
                case PodcastEpisodeNamingStyle.UrlFileName:
                    return Resource.String.feed_naming_style_urlfilename;
                case PodcastEpisodeNamingStyle.UrlFileNameAndPublishDateTime:
                    return Resource.String.feed_naming_style_urlfilenameandpublishdatetime;
                case PodcastEpisodeNamingStyle.UrlFileNameFeedTitleAndPublishDateTime:
                    return Resource.String.feed_naming_style_urlfilenamefeedtitleandpublishdatetime;
                case PodcastEpisodeNamingStyle.UrlFileNameFeedTitleAndPublishDateTimeInfolder:
                    return Resource.String.feed_naming_style_urlfilenamefeedtitleandpublishdatetimeinfolder;
                case PodcastEpisodeNamingStyle.EpisodeTitle:
                    return Resource.String.feed_naming_style_episodetitle;
                case PodcastEpisodeNamingStyle.EpisodeTitleAndPublishDateTime:
                    return Resource.String.feed_naming_style_episodetitleandpublishdatetime;
            }
            return Resource.String.unknown;
        }

        private object GetNamingStyleAdditionalText(PodcastEpisodeNamingStyle namingstyle)
        {
            switch (namingstyle)
            {
                case PodcastEpisodeNamingStyle.UrlFileNameFeedTitleAndPublishDateTimeInfolder:
                    return $"{SEPERATOR}{ResourceProvider.GetString(Resource.String.naming_style_UrlFileNameFeedTitleAndPublishDateTimeInfolder_extra)}";
                case PodcastEpisodeNamingStyle.UrlFileName:
                case PodcastEpisodeNamingStyle.UrlFileNameAndPublishDateTime:
                case PodcastEpisodeNamingStyle.UrlFileNameFeedTitleAndPublishDateTime:
                    return $"{SEPERATOR}{ResourceProvider.GetString(Resource.String.naming_style_Url_extra)}";
            }
            return string.Empty;
        }

        private int GetDownloadStratagyTextId(PodcastEpisodeDownloadStrategy stratagy)
        {
            switch (stratagy)
            {
                case PodcastEpisodeDownloadStrategy.All:
                    return Resource.String.feed_download_stratagy_all;
                case PodcastEpisodeDownloadStrategy.HighTide:
                    return Resource.String.feed_download_stratagy_hightide;
                case PodcastEpisodeDownloadStrategy.Latest:
                    return Resource.String.feed_download_stratagy_latest;
            }
            return Resource.String.unknown;
        }

        private string GetDownloadStrategyAdditionalText(PodcastEpisodeDownloadStrategy stratagy)
        {
            switch (stratagy)
            {
                case PodcastEpisodeDownloadStrategy.All:
                    return $"{SEPERATOR}{ResourceProvider.GetString(Resource.String.xtra_download_strategy_all)}";
                case PodcastEpisodeDownloadStrategy.HighTide:
                    return $"{SEPERATOR}{ResourceProvider.GetString(Resource.String.xtra_download_strategy_high_tide)}";
                case PodcastEpisodeDownloadStrategy.Latest:
                    return $"{SEPERATOR}{ResourceProvider.GetString(Resource.String.xtra_download_strategy_latest)}";
            }
            return string.Empty;
        }

        public string GetDownloadStratagyText(PodcastEpisodeDownloadStrategy strategy)
        {
            return ResourceProvider.GetString(GetDownloadStratagyTextId(strategy));
        }

        public string GetDownloadStratagyTextLong(PodcastEpisodeDownloadStrategy strategy)
        {
            return $"{GetDownloadStratagyText(strategy)}{GetDownloadStrategyAdditionalText(strategy)}";
        }

        public string GetNamingStyleText(PodcastEpisodeNamingStyle namingstyle)
        {
            return ResourceProvider.GetString(GetNamingStyleTextId(namingstyle));
        }

        public string GetNamingStyleTextLong(PodcastEpisodeNamingStyle namingstyle)
        {
            return $"{namingstyle}{SEPERATOR}{GetNamingStyleText(namingstyle)}{GetNamingStyleAdditionalText(namingstyle)}";
        }

        public string GetCustomOrNamedIntValue(int namedTextId, int namedValue, int customFormatId, int value)
        {
            if (value == namedValue)
            {
                return ResourceProvider.GetString(namedTextId);
            }
            else
            {
                return string.Format(ResourceProvider.GetString(customFormatId), value);
            }
        }

        public string GetFeedOverrideSummary(IPodcastInfo podcastInfo)
        {
            if (podcastInfo == null || podcastInfo.Feed == null)
            {
                return ResourceProvider.GetString(Resource.String.feed_no_feed);
            }

            IList<string> overrideTokens = new List<string>();
            if (podcastInfo.Feed.DownloadStrategy.IsSet)
            {
                overrideTokens.Add(ResourceProvider.GetString(Resource.String.feed_override_strategy));
            }
            if (podcastInfo.Feed.NamingStyle.IsSet)
            {
                overrideTokens.Add(ResourceProvider.GetString(Resource.String.feed_override_naming_style));
            }
            if (podcastInfo.Feed.MaximumDaysOld.IsSet)
            {
                overrideTokens.Add(ResourceProvider.GetString(Resource.String.feed_override_max_days_old));
            }
            if (podcastInfo.Feed.DeleteDownloadsDaysOld.IsSet)
            {
                overrideTokens.Add(ResourceProvider.GetString(Resource.String.feed_override_delete_days_old));
            }
            if (podcastInfo.Feed.MaximumNumberOfDownloadedItems.IsSet)
            {
                overrideTokens.Add(ResourceProvider.GetString(Resource.String.feed_override_max_items));
            }
                                ;
            if (overrideTokens.Count > 0)
            {
                var overrideStr = string.Join(",", overrideTokens);
                return $"{ResourceProvider.GetString(Resource.String.feed_overrides)} {overrideStr}"
                ;
            }
            else
            {
                return ResourceProvider.GetString(Resource.String.feed_no_overrides);
            }
        }

        private string AddDefaultPrefix(bool isSet, string currentValueStr)
        {
            if (isSet)
            {
                return currentValueStr;
            }
            else
            {
                return $"{ResourceProvider.GetString(Resource.String.feed_uses_default_prefix)} {currentValueStr}";
            }
        }

        public string GetDefaultableCustomOrNamedIntValue(int namedTextId, int namedValue, int customFormatId, IDefaultableItem<int> item)
        {
            var currentValueStr = GetCustomOrNamedIntValue(namedTextId, namedValue, customFormatId, item.Value);
            return AddDefaultPrefix(item.IsSet, currentValueStr);
        }

        public string GetDefaultableNamingStyleTextLong(IDefaultableItem<PodcastEpisodeNamingStyle> namingStyle)
        {
            var currentValueStr = GetNamingStyleTextLong(namingStyle.Value);
            return AddDefaultPrefix(namingStyle.IsSet, currentValueStr);
        }

        public string GetDefaultableDownloadStratagyTextLong(IDefaultableItem<PodcastEpisodeDownloadStrategy> downloadStrategy)
        {
            var currentValueStr = GetDownloadStratagyTextLong(downloadStrategy.Value);
            return AddDefaultPrefix(downloadStrategy.IsSet, currentValueStr);
        }

        private string GetPlaylistFormatAdditionalText(PlaylistFormat playlistFormat)
        {
            switch (playlistFormat)
            {
                case PlaylistFormat.ASX:
                    return $"{SEPERATOR}{ResourceProvider.GetString(Resource.String.xtra_playlist_format_asx)}";
                case PlaylistFormat.M3U:
                    return $"{SEPERATOR}{ResourceProvider.GetString(Resource.String.xtra_playlist_format_m3u)}";
                case PlaylistFormat.WPL:
                    return $"{SEPERATOR}{ResourceProvider.GetString(Resource.String.xtra_playlist_format_wpl)}";
            }
            return string.Empty;
        }

        public string GetPlaylistFormatTextLong(PlaylistFormat playlistFormat)
        {
            return $"{playlistFormat}{GetPlaylistFormatAdditionalText(playlistFormat)}";
        }

        private object GetDiagOutputAdditionalText(DiagnosticOutputLevel diagnosticOutputLevel)
        {
            switch (diagnosticOutputLevel)
            {
                case DiagnosticOutputLevel.None:
                    return $"{SEPERATOR}{ResourceProvider.GetString(Resource.String.xtra_diag_output_none)}\n{FileSystemHelper.GetApplicationFirstExternalPath()}";
                case DiagnosticOutputLevel.Verbose:
                    return $"{SEPERATOR}{ResourceProvider.GetString(Resource.String.xtra_diag_output_verbose)}\n{FileSystemHelper.GetApplicationFirstExternalPath()}";
            }
            return string.Empty;
        }

        public string GetDiagOutputTextLong(DiagnosticOutputLevel diagnosticOutputLevel)
        {
            return $"{diagnosticOutputLevel}{GetDiagOutputAdditionalText(diagnosticOutputLevel)}";
        }

        public string GetDayOffsetAsDateString(int offset)
        {
            var date = TimeProvider.UtcNow.AddDays(offset);
            return date.ToString("d MMM yyyy", CultureInfo.InvariantCulture);
        }

    }
}