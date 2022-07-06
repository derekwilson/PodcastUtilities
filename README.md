# PodcastUtilities #

PodcastUtilities are a set of utilities designed to help manage digital media on removable drives. Utilities are provided to Download Podcasts, Sync Podcasts and Generate Playlists

### In this repository ###

* The ```_PreBuildPackages``` folder has a compiled ZIP file if you just want to run the
  - the folder `net35` contains a build to used on Windows XP, Vista, 7, 8, 10 and .NET framework v3.5  app
  - the folder `netcoreapp2.1` contains a build for .NET Core for Linux, MacOS and Windows or for use with Mono
* Tools section contains installers for the open source tools used to build the app
* The rest of the repository contains the C# code, scripts and documentation to build the app

### Requirements to compile the code ###

* VisualStudio 2017 (any version including the free versions)
* In the past I did check that SharpDevelop worked so that might also be an option
* NUnit, MSBuild Community Tasks (installers are in the tools folder)
* Run the tests in NUnit UI, .NET Framework v3.5 tests are no longer supported by VisualStudio  - also there is a [TeamCity project](http://teamcity.codebetter.com/viewType.html?buildTypeId=bt1062) that runs the tests

### Building the project

* To build all the .NET Framework assemblies - in VS 2017 select Build -> Batch Build and select all the Release targets and rebuild
* To build all the .NET Core assemblies - from the command line run `PublishAll.bat` from the root of the project
* To package the ZIP - from the command line run `BuildAll.cmd` (after installing the community tasks)

Sometimes when running the `BuildAll` command on Windows 10 you may get this error

```
.\Build Scripts\build.xml(63,5): error MSB3231: Unable to remove directory "Build Scripts\..\CurrentBuild". The directory is not empty
```

I am not sure why it happens however if you run the command a second time it does work.