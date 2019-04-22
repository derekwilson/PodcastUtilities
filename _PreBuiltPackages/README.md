# PodcastUtilities #

PodcastUtilities are a set of utilities designed to help manage digital media on removable drives. Utilities are provided to Download Podcasts, Sync Podcasts and Generate Playlists

This folder contains prebuild packages for Windows, the file name has the date the package was built, if in doubt take the latest dated package. Download, unzip, read the documentation and away you go 

### Benifits ###

* Portable
PodcastUtilities is a “portable” application, in that it does not need to be installed and can be run from any drive type, local network or removable flash drive
* Cross Platform
Podcast Utilities can be run on Windows, Linus or Mac machines by using the .NET Core cross platform version of the utilities.
* Configurable
Almost any synchronisation solution can be configured, for example copy 3 of this podcast, all of these podcasts, in this order, and leave a certain amount of space on the device. The number of concurrent downloads can easily be configured.
* Lightweight
The application does not use large amounts of memory in a system tray icon all the time its not being used, it only uses system resources when its being run.
* Support for UMS and MTP
PodcastUtilities supports USB Mass Storage (UMS) devices, where the device appears as a drive letter in Windows Explorer like a flash drive, as well as MTP devices where the device appears as a portable device, for example many phones. MTP is not supported in the .NET Core version.
* Scriptable
PodcastUtilities lends itself to being run as a scheduled task or from a script. The API is “headless” and can easily be called and used from other applications
* Open Source
The source is open and available
* Free
There is no cost to using Podcast Utilities for any use.
* No need for an iTunes account
Podcasts promoted on iTunes can be downloaded however there is no need to have an iTunes account.


### Supported Platforms ###

* .NET Framework v3.5
The files are in a folder called `net35`. It will run on Windows XP, Vista, Windows 7, 8, 10. The framework is not part of the operating system for later versions of Windows 10 and will need to be installed seperatly.

See https://docs.microsoft.com/en-us/dotnet/framework/install/dotnet-35-windows-10

* .NET Core v2.1
The files are in a folder called `netcoreapp2.1`. It will run on any platform that is supported by the .NET Core runtime, it includes Windows, Linux and Mac. You will need to install the runtime before using the utilities.

See https://dotnet.microsoft.com/download



