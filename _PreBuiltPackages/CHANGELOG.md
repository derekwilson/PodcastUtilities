# v3.0.2.7 - 15 April 2022
1. FIX: ControlFile loading and saving was leaving the file open because `Dispose()` was not being called
1. FIX: Added missing `PostDownloadCommand` property to `IPodcastInfo`
1. FIX: Added missing `GetDiagnosticOutput` and `GetDiagnosticRetainTemporaryFiles` to `IReadOnlyControlFile`
1. FIX: `postdownloadcommand` element was being written twice by `SaveToFile`, one of them an invalid format which was causing files to become corrupt when written.
1. FIX: Changed all calls to `EventHandler<>.Invoke()` to be the new `handler?.Invoke(this,args)` as it can avoid some race conditions
1. FIX: `PodcastEpisodeNamingStyle.UrlFileNameFeedTitleAndPublishDateTimeInfolder` was always using the windows path separator in the folder name, it now follows the OS where it is running for the separator
1. `Podcasts` property of the ControlFile changed from `IList<PodcastInfo>` to be `IList<IPodcastInfo>` as it enables mocking and testing
1. `SyncItem` objects returned by the `IEpisodeFinder` now contain a `Guid` ID.
1. `StatusUpdateEventArgs` now has a flag to indicate if the task is complete as well as a reference to the `SyncItem` if available
1. Added these chars to the invalid filename chars ('?', ':', '\'', '’') as Google's crappy MediaStore implementation treats them as invalid (in some folders)
1. Added ability to register an already created object into the `IIocContainer` by calling `Register<TService>(TService instance)`
1. Created `IGenerator` and added the playlist generator into the IocContainer
1. Added the ability to generate a playlist to an arbitrary folder to `IGenerator`
1. Playlists are now sorted

# v3.0.0.2 - 24 September 2021

1. First version deployed through Chocolatey
1. Updated user docs to describe installing through Chocolatey
1. Tweaked `app.config` to enable all console apps to run under .NET Framework runtime version 2 *or* version 4 depending what is installed. This means that you no longer are prompted to install .NET Framework 3.5 when running on Windows 10 and 11.
1. The utilities console apps now prints out the environment information when displaying help

# v3.0.0.0 - 23 April 2019

1. Updated solution to VS 2017
1. Ported the core assemblies and console apps to .NET Core. Supporting .NET Standard v2 and .NET Core v2.1
1. .NET Core version should be identical to the v2.2.2.0 .NET Framework (Windows only build). The only differences being
    -  MTP - we did this by using PInvoke this obviously depends on the platform so I have excluded it. We would only ever be able to get it to work on Windows and you might as well use the net35 build
    -  Permon counters - not supported in the BCL - this may change in future
    -  Tests - NUnit is supported however Rhino mocks are not
    -  WPF - used in the App, this isn’t a big loss as we never shipped it in the main ZIP anyway. WPF is due to be a part of .NET Core v3 released this year.
    -  LinFu - not supported on .NET Core, I have provided an implementation of our IoC Container that uses Microsoft.Extensions.DependencyInjection (this is available .NET Framework but guess what - not v3.5 we need at least 4.6.1)
1. .NET Core version has been tested on 
    - .NET Core on Windows, Unbuntu on x86, Ubuntu on ARM v8 and Mac
    - Mono on Ubuntu on x86, Ubuntu on ARM V8 and Mac

# v2.2.2.0 - 19 June 2018 

1. Prevent crash if TLS1.1 and TLS 2.0 not available

# v2.2.1.0 - 12 June 2018 

1. Workaround for servers that no longer support TLS1.0

# v2.2.0.0 - 28 February 2018 

1. Added support for M3U playlist format

# v2.1.1.0 - 22 February 2018

1. FIX: Fixed `AccessViolationException` with Windows 10 Creators Update 1703: `IStream.Write` no longer allows `NULL` for `pcbWritten`
1. Added support for `playlistPathSeparator`

# v2.1.0.8 - 2 July 2016

1. Updated solution to VS 2015

# v2.1.0.7 - 8 December 2015

1. Allowed invalid suffix UTC in the RFC822 Date as some sites use this suffix

# v2.1.0.6 - 18 February 2015

1. Fixed a bug where automatically deleting a folder would remove the state.xml file
1. Stopped creating a download folder unless it is needed
1. Added ability to delete empty folders to SyncPodcasts
1. Added ability to purge empty folders
1. Added `DeleteEmptyFolder` config setting

# v2.1.0.3 - 19 December 2013

1. Fixed an issue where if there was an error getting the free space on the destination drive then it would stop the copying. This prevented the destination of a sync from being a relative path, a UNC path or an MTP path

# v2.1.0.1 - 28 July 2013

1. Added support for MTP devices and path schemas

# v2.0.0.7 - 9 September 2012

1. FIX: Fixed issues with `DownloadPodcasts` and `SyncPodcasts` if the podcasts were stored and accessed via a UNC pathname in the sourceRoot of the control file.
1. Added support for `{exefolder}` token in post download commands
1. Added `PostDownloadCommand` into config and supported args and cwd, changed from a string to be an `ITokenisedCommand`
1. Added `PerfmonCountersInstaller`
1. Made `ReadWriteControlFile` support `ICloneable`, test cloning to test `XmlSerialisation` read and write

# v2.0.0.0 - 6 March 2012

1. Added `MaximumNumberOfFiles` (to sync) into the global defaults for a PodcastInfo
1. Made `MaximumNumberOfFiles` defaultable
1. Added diagnostics section to global config, implemented verbose and retain temp files
1. Update docs for download to media player strategy, updated API CHM and example control file
