using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;
using PodcastUtilities.Common.Playlists;

namespace PodcastUtilities.Common.Configuration
{
    /// <summary>
    /// this object represents the xml control file
    /// </summary>
    public interface IReadWriteControlFile : IReadOnlyControlFile, IXmlSerializable
    {
        /// <summary>
        /// level of diagnostic output
        /// </summary>
        void SetDiagnosticOutput(DiagnosticOutputLevel level);

        /// <summary>
        /// set to retain intermediate files
        /// </summary>
        void SetDiagnosticRetainTemporaryFiles(bool retainFiles);

        /// <summary>
        /// pathname to the root folder to copy from when synchronising
        /// </summary>
        void SetSourceRoot(string value);

        /// <summary>
        /// pathname to the destination root folder
        /// </summary>
        void SetDestinationRoot(string value);

        /// <summary>
        /// filename and extension for the generated playlist
        /// </summary>
        void SetPlaylistFileName(string value);

        /// <summary>
        /// the format for the generated playlist
        /// </summary>
        void SetPlaylistFormat(PlaylistFormat value);

        /// <summary>
        /// free space in MB to leave on the destination device when syncing
        /// </summary>
        void SetFreeSpaceToLeaveOnDestination(long value);

        /// <summary>
        /// free space in MB to leave on the download device - when downloading
        /// </summary>
        void SetFreeSpaceToLeaveOnDownload(long value);

        /// <summary>
        /// maximum number of background downloads
        /// </summary>
        void SetMaximumNumberOfConcurrentDownloads(int value);

        /// <summary>
        /// number of seconds to wait when trying a file conflict
        /// </summary>
        void SetRetryWaitInSeconds(int value);

        /// <summary>
        /// persist the control file to disk
        /// </summary>
        /// <param name="fileName"></param>
        void SaveToFile(string fileName);
    }
}