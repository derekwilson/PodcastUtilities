# PodcastUtilities #

PodcastUtilities are a set of utilities designed to help manage digital media on removable drives. Utilities are provided to Download Podcasts, Sync Podcasts and Generate Playlists

### In this repository ###

* The ```_PreBuildPackages``` folder has a compiled ZIP file for use on Windows XP, Vista, 7, 8, 10 and .NET framework v3.5 if you just want to run the app
* Tools section contains installers for the open source tools used to build the app
* The rest of the repository contains the C# code, scripts and documentation to build the app

### Requirements to compile the code ###

* VisualStudio 2010 (any version including the free versions)
* You can probably use later versions but that is untried
* In the past I did check that SharpDevelop worked so that might also be an option
* NUnit, MSBuild Community Tasks (installers are in the tools folder)
* Run the tests in VisualStudio - also there is a [TeamCity project](http://teamcity.codebetter.com/viewType.html?buildTypeId=bt1062) that runs the tests
* To build from the command line run BuildAll (after installing the community tasks)