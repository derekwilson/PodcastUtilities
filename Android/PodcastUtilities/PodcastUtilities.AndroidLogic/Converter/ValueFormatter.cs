using Android.Media;
using PodcastUtilities.AndroidLogic.Logging;
using PodcastUtilities.AndroidLogic.Utilities;
using PodcastUtilities.Common.Configuration;

namespace PodcastUtilities.AndroidLogic.Converter
{
    public interface IValueFormatter
    {
        string GetNamingStyleText(PodcastEpisodeNamingStyle namingstyle);
        string GetNamingStyleTextLong(PodcastEpisodeNamingStyle namingstyle);
        string GetDownloadStratagyText(PodcastEpisodeDownloadStrategy strategy);
        string GetDownloadStratagyTextLong(PodcastEpisodeDownloadStrategy strategy);
        string GetCustomOrNamedIntValue(int namedTextId, int namedValue, int customFormatId, int value);
    }

    public class ValueFormatter : IValueFormatter
    {
        private ICrashReporter CrashReporter;
        private ILogger Logger;
        private IResourceProvider ResourceProvider;

        public ValueFormatter(
            ICrashReporter crashReporter,
            ILogger logger,
            IResourceProvider resourceProvider)
        {
            CrashReporter = crashReporter;
            Logger = logger;
            ResourceProvider = resourceProvider;
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
            var fred = GetDownloadStrategyAdditionalText(strategy);
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
    }
}