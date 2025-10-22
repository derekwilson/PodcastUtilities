# PodcastUtilities #

PodcastUtilities are a set of utilities designed to help manage digital media on removable drives. Utilities are provided to Download Podcasts, Sync Podcasts and Generate Playlists and to manage an offline cache of media.

The CLI tools run on Windows, Linux, and MacOS

There is an incomplete GUI that runs on Windows and there is a complete Android app that you can build or get from the Amazon Appstore.

### In this repository ###

* The `_PreBuildPackages` folder has a compiled ZIP file of the main projects output assemblies
  - the folder `net35` contains a build to used on Windows XP, Vista, 7, 8, 10 and .NET framework v3.5  app
  - the folder `netcoreapp3.1` contains a build for .NET Core for Linux, MacOS and Windows or for use with Mono
* The `Android\Support\_PreBuiltPackages/` folder has compiled APKs for side-loading onto an Android device
* Tools section contains installers for the open source tools used to build the apps
* The rest of the repository contains the C# code, scripts and documentation to build the apps

### Requirements to compile the code ###

* VisualStudio 2022 (any version including the free versions)
* In the past I did check that SharpDevelop worked so that might also be an option
* NUnit, MSBuild Community Tasks (installers are in the tools folder)
* To run the tests in VS you will need to [download the developer pack](https://dotnet.microsoft.com/en-us/download/visual-studio-sdks?cid=msbuild-developerpacks) for .NETFramework, currently v4.6.2

### Building the solution

Open the solution file `PodcastUtilities.Multiplatform.sln`

* To build all the .NET Framework assemblies - in VisualStudio select Build -> Batch Build and select all the Release targets and rebuild
* To build all the .NET Core assemblies - from the VisualStudio developer command prompt run `PublishAll.bat` from the root of the project
* To package the ZIP - from the command line run `BuildAll.cmd` (after installing the community tasks)

Sometimes when running the `BuildAll` command on Windows 10 you may get this error

```
.\Build Scripts\build.xml(63,5): error MSB3231: Unable to remove directory "Build Scripts\..\CurrentBuild". The directory is not empty
```

I am not sure why it happens however if you run the command a second time it does work.

### The Projects

The main solution `PodcastUtilities.Multiplatform.sln` contains these projects

#### Core projects

| Project                                           | .NETCore         | .NETFramework | Notes
|:--------------------------------------------------|:-----------------|:--------------|-------
| PodcastUtilities.Common.Multiplatform             | .NETStandard v2  | 3.5           | Core functionality
| PodcastUtilities.Ioc.Multiplatform                | .NETStandard v2  | 3.5           | Optional IoC container for the core assembly
| PodcastUtilities.Common.Multiplatform.Tests       | 3.1              | 4.6.2         | Core tests, NUnit/Moq
| PodcastUtilities.Common.Tests                     |                  | 3.5           | Core tests, NUnit/Rhino.Mocks
| PodcastUtilities.Integration.Tests.Multiplatform  | 3.1              | 3.5           | Integration tests, to be run on target

The test projects can be run from VS2022 Test Explorer, older versions of VS cannot run the tests targetting .NET 3.5

The tests targetting .NETCore can be run from the command line like this

```
dotnet test --framework netcoreapp3.1
```

#### CLI projects

| Project                                           | .NETCore         | .NETFramework | Notes
|:--------------------------------------------------|:-----------------|:--------------|-------
| DownloadPodcasts.Multiplatform                    | 3.1              | 3.5           | Downloader
| GeneratePlaylist.Multiplatform                    | 3.1              | 3.5           | Playlist generator
| PurgePodcasts.Multiplatform                       | 3.1              | 3.5           | Purger
| SyncPodcasts.Multiplatform                        | 3.1              | 3.5           | Sync

#### Windows only projects

| Project                                           | .NETCore         | .NETFramework | Notes
|:--------------------------------------------------|:-----------------|:--------------|-------
| PodcastUtilities.App                              |                  | 3.5           | Windows GUI
| PodcastUtilities.Presentation                     |                  | 3.5           | logic for Windows GUI
| PodcastUtilities.Presentation.Tests               |                  | 3.5           | tests for Windows GUI, NUnit/Rhino.Mocks
| PerfmonCountersInstaller                          |                  | 3.5           | Installer for the perfmon counters
| PodcastUtilities.PortableDevices                  |                  | 3.5           | MTP support
| PodcastUtilities.PortableDevices.Tests            |                  | 3.5           | Tests MTP support, NUnit/Rhino.Mocks

### Android Projects

A separate solution `Android\PodcastUtilities\PodcastUtilities.sln` contains these projects for use on Android devices

| Project                                           | .NETCore         | .NETFramework | Notes
|:--------------------------------------------------|:-----------------|:--------------|-------
| PodcastUtilities                                  | 9                |               | Android app
| PodcastUtilities.AndroidLogic                     | 9                |               | Logic for Android app
| PodcastUtilities.AndroidTests                     | 9                |               | Tests for Android app, NUnit/FakeItEasy

The test project compiles to an Android app and should be run on a device.


