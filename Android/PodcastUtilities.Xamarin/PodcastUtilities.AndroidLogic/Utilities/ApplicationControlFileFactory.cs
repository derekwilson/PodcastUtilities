using PodcastUtilities.Common.Configuration;
using System;
using System.Xml;

namespace PodcastUtilities.AndroidLogic.Utilities
{
    public interface IApplicationControlFileFactory
    {
        IReadWriteControlFile CreateControlFile(XmlDocument xml);
        IReadWriteControlFile CreateEmptyControlFile();
    }
    public class ApplicationControlFileFactory : IApplicationControlFileFactory
    {
        private IResourceProvider ResourceProvider;

        public ApplicationControlFileFactory(
            IResourceProvider resourceProvider
            )
        {
            ResourceProvider = resourceProvider;
        }

        private void SanitiseConfiguration(IReadWriteControlFile file)
        {
            // PU allows for things in a control file that does not make much sense for this app
            int index = 0;
            foreach (var podcastInfo in file.GetPodcasts())
            {
                // make sure each podcastinfo has a folder specified or all episodes end up in the same folder
                if (string.IsNullOrWhiteSpace(podcastInfo.Folder))
                {
                    podcastInfo.Folder = string.Format(ResourceProvider.GetString(Resource.String.default_podcast_folder), index);
                }
                index++;
            }
        }
        public IReadWriteControlFile CreateControlFile(XmlDocument xml)
        {
            var file = new ReadWriteControlFile(xml);
            SanitiseConfiguration(file);
            return file;
        }

        public IReadWriteControlFile CreateEmptyControlFile()
        {
            var file = new ReadWriteControlFile();
            ApplyAppDefaultConfig(file);
            return file;
        }

        private void ApplyAppDefaultConfig(ReadWriteControlFile file)
        {
            // make a guess about the cache root
            file.SetSourceRoot(ResourceProvider.GetString(Resource.String.cache_root_option_phone));
            // the file system uses unix style seperators
            file.SetPlaylistPathSeparator("/");
            // set a reasonable default for playlists
            file.SetPlaylistFormat(Common.Playlists.PlaylistFormat.M3U);
            file.SetPlaylistFileName("podcasts.m3u");
            // feed defaults that work for me
            file.SetDefaultDownloadStrategy(PodcastEpisodeDownloadStrategy.HighTide);
            file.SetDefaultNamingStyle(PodcastEpisodeNamingStyle.EpisodeTitleAndPublishDateTime);
            file.SetDefaultMaximumDaysOld(31);
        }
    }
}