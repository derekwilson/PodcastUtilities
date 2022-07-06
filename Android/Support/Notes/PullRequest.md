# What has been done

Using the PodcastUtilities .NET Standard DLLs I have produced a version that runs on Android.

The changes fall into the following groups

1. Changes to the PodcastUtilities .NET Standard DLLs
1. Create a PodcastUtilitiesPOC app to prove out the concept
1. Create a PodcastUtilities app and tests for release to users

## PodcastUtilities .NET Standard DLLs

All the fixes to `PodcastUtilities.Common` are related to its use in a multi threaded UI that reads and writes control files, such as the android client. The existing CLI UI will not be affected by the issues being fixed.

Of the enhancements much the same can be said except for the new invalid filename chars and also the sorting of playlists.

v3.0.2.7 The changes here are

1. FIX: ControlFile loading and saving was leaving the file open because `Dispose()` was not being called
1. FIX: Added missing `PostDownloadCommand` property to `IPodcastInfo`
1. FIX: Added missing `GetDiagnosticOutput` and `GetDiagnosticRetainTemporaryFiles` to `IReadOnlyControlFile`
1. FIX: `postdownloadcommand` element was being written twice by `SaveToFile`, one of the an invalid format which was causing files to become corrupt when written.
1. FIX: Changed all calls to `EventHandler<>.Invoke()` to be the new `handler?.Invoke(this,args)` as it can avoid some race conditions
1. FIX: `PodcastEpisodeNamingStyle.UrlFileNameFeedTitleAndPublishDateTimeInfolder` was always using the windows path separator in the folder name, it now follows the OS where it is running for the separator
1. `Podcasts` property of the ControlFile changed from `IList<PodcastInfo>` to be `IList<IPodcastInfo>` as it enables mocking and testing
1. `SyncItem` objects returned by the `IEpisodeFinder` now contain a `Guid` ID.
1. `StatusUpdateEventArgs` now has a flag to indicate if the task is complete as well as a reference to the `SyncItem` if available
1. Added these chars to the invalid filename chars ('?', ':', '\'', '’') as Google's crappy MediaStore implementation treats them as invalid
1. Added ability to register an already created object into the `IIocContainer` by calling `Register<TService>(TService instance)`
1. Created `IGenerator` and added the playlist generator into the IocContainer
1. Added the ability to generate a playlist to an arbitrary folder to `IGenerator`
1. Playlists are now sorted

## PodcastUtilitiesPOC

Like the name suggests this solution tests the techniques needed to get PodcastUtilities working on Android. The POC

1. Uses VS2019
1. Targets Android 11
1. Uses xUnit/FakeItEasy
1. Uses NLog

Example techniqies include

1. ViewModels and factories
1. Unit Tests
1. LiveData
1. C# EventHandler
1. Custom Icons
1. Logging
1. Building Debug and Release and switching package names
1. Signing release APKs
1. Using `LocalOnly` for signing keys
1. Custom Views
1. Permissions
1. IoC, for `PodcastUtilities.Common.DLL` and also Android objects 
1. Calling `PodcastUtilities.Common.DLL`

## PodcastUtilities

This solution contains the release code for PodcastUtilities Android client. It

1. Uses VS2022
1. Targets Android 12
1. Uses nUnit/FakeItEasy
1. Uses NLog
1. Uses Crashlytics
1. Uses Firebase Analytics

Build instructions are in the readme of the solution.
