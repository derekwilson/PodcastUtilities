## Android version of PodcastUtilities

This folder contains the code for the Android version of the project.

- `PodcastUtilities` is the code for the app from the app store
- `PodcastUtilitiesPOC` is the proof of concept code, it is a place for experements, it has never been released to the app store.
- `Reference` is where the projects pick up any direct DLL assembly references. For example `PodcastUtilities.Common.dll`. We reference an assembly to allow for different versions of the build tools for the common code and the android code.

### PodcastUtilitiesPOC

Is built using VS2019

#### Install VS2019

And also install Xamarin Android

#### Enable powershell

The package name updating mechanism means that you need to enable powershell commands like this

```
Set-ExecutionPolicy RemoteSigned
```

See

https://stackoverflow.com/questions/27798862/is-it-possible-to-specify-package-name-dynamically-during-build


### PodcastUtilities

Is built using VS2022

#### Install VS2022

And also install Xamarin Android

Powershell is configured by the project scripts, there is no manual process

#### Building Debug Builds

1. Open BS2022
1. Rebuild Solution
1. Connect Device
1. Select the device you want to deploy to in the toolbar
1. Select `PodcastUtilities`
1. Select Build -> Deploy

Note: it will fail if the app is already running

#### Building the Release Build

Note: You need to have the folder `PodcastUtilities\Android\PodcastUtilities\LocalOnly`. Its not in the repo you will need to get it from one of the team, if you dont have it then you can still build the debug builds but you cannot sign the release builds.

##### Building a release AAB

If you intend to deploy the build using Google Play Store you need to build an AAB. The AAB is signed with the key from `LocalOnly`, this will be used as an upload key on the play store the then Google will resign the app as it is downloaded.

1. Open a developer command prompt for VS2022
1. Goto `Android\PodcastUtilities`
1. Run `BuildReleaseAAB.bat`
1. The `aab` will be copied to `Android\Support\_PreBuiltPackages` and will be called `com.andrewandderek.podcastutilities-Signed.aab`
1. Goto `Android\Support\_PreBuiltPackages`
1. Run `GenerateApks.bat`
1. This will create a `universal.apk`
1. Connect a test device
1. Run `InstallUniversalApk.bat`

##### Building a release APK

If you are intending to deploy the app by having the user download it from GitHub then you must build an APK, as phones cannot install AAB's (thanks Google). The APK will be signed using the key in `LocalOnly`, this will be the app signing key as the user will install the APK directly, the play store is not involved. 

1. Open a developer command prompt for VS2022
1. Goto `Android\PodcastUtilities`
1. Run `BuildReleaseAPK.bat`
1. The `apk` will be copied to `Android\Support\_PreBuiltPackages` and will be called `com.andrewandderek.podcastutilities-Signed.apk`
1. Goto `Android\Support\_PreBuiltPackages`
1. Connect a test device
1. Run `InstallReleaseApk.bat`

##### Notes on installing release builds

You cannot upgrade from an AAB to an APK install, or visa versa, as they are signed with different keys. The AAB is signed by Google and the APK is signed by the developer.

Sometimes when running `BuildReleaseAPK.bat` it will generate a file called `com.andrewandderek.podcastutilities.debug-Signed.aab`, and the copy operation will fail, if this happens just run the script again.
