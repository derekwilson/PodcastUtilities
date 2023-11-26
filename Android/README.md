## Android version of PodcastUtilities

This folder contains the code for the Android version of the project.

- `PodcastUtilities` is the code for the app from the app store
- `PodcastUtilitiesPOC` is the proof of concept code, it is a place for experements, it has never been released to the app store.
- `Reference` is where the projects pick up any direct DLL assembly references. For example `PodcastUtilities.Common.dll`. We reference an assembly to allow for different versions of the build tools for the common code and the android code.
- `Support` holds the prebuilt release archive as well as assets for play store releases


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

1. Open VS2022
1. Rebuild Solution
1. Connect Device
1. Select the device you want to deploy to in the toolbar
1. Select `PodcastUtilities`
1. Select Build -> Deploy

Note: it will fail if the app is already running

Note: If you dont have the folder `PodcastUtilities\Android\PodcastUtilities\LocalOnly` then you will need to provide an AppCenter secrets key in `AndroidApplication.cs`

```
public override void OnCreate()
{
    Log.Debug(LOGCAT_TAG, $"AndroidApplication:OnCreate SDK == {Android.OS.Build.VERSION.SdkInt}, {(int)Android.OS.Build.VERSION.SdkInt}");
    Log.Debug(LOGCAT_TAG, $"AndroidApplication:OnCreate PackageName == {this.PackageName}");

    AppCenter.Start(Secrets.APP_CENTER_SECRET, typeof(Analytics), typeof(Crashes));
```


#### Building the Release Build

Note: You need to have the folder `PodcastUtilities\Android\PodcastUtilities\LocalOnly`. Its not in the repo you will need to get it from one of the team, if you dont have it then you can still build the debug builds but you cannot sign the release builds.

#### Package names

The deployment mechanisms (Sideload or GooglePlay) are not interchangeable as different signing keys are used for each mechanism. For GooglePlay Google will re-sign the AAB as it is downloaded and for Sideload the signing key is in `LocalOnly`. For that reason users cannot install using different mechanisms even if the package names are the same (in fact this requires them to uninstall and reinstall rather than upgrade).

To keep this manageable we use different package names for each mechanism.

| Deployment Mechanism | Configuration | Packagename |
| -------------------- | ------------- | ----------- |
| GooglePlay           | Release       | com.andrewandderek.podcastutilities
| GooglePlay           | Debug         | com.andrewandderek.podcastutilities.debug
| Sideload             | Release       | com.andrewandderek.podcastutilities.sideload
| Sideload             | Debug         | com.andrewandderek.podcastutilities.sideload.debug 

The packagename is displayed with the version information on the settings screen in the app. Its also the folder name below `sdcard/Android/data` where the logs and cached files are stored.

The `.debug` suffix is automatically applied with scripts as we build the app, the presence of `sideload` needs to be manually edited into the manifest as its not often changed. As Google have declined to distribute the app it will usually be present.

##### Building a release AAB

If you intend to deploy the build using Google Play Store you need to build an AAB. The AAB is signed with the key from `LocalOnly`, this will be used as an upload key on the play store the then Google will resign the app as it is downloaded.

1. In VS Select Project Properties for `PodcastUtilities` -> Android Manifest enter the correct VersionName and VersionNumber
1. Open a developer command prompt for VS2022
1. Goto `Android\PodcastUtilities`
1. Run `BuildReleaseAAB.bat`
1. The `aab` will be copied to `Android\Support\CurrentBuild` and will be called `com.andrewandderek.podcastutilities-Signed.aab`
1. Goto `Android\Support\CurrentBuild`
1. Run `GenerateApks.bat`
1. This will create a `universal.apk`
1. Connect a test device
1. Run `InstallUniversalApk.bat`

##### Building a release APK

If you are intending to deploy the app using Amazon App Store or by having the user download it from GitHub then you must build an APK, as phones cannot install AAB's (thanks Google). The APK will be signed using the key in `LocalOnly`, this will be the app signing key as the user will install the APK directly, the play store is not involved. 

Note: Sometimes when the `BuildReleaseAPK.bat` ,is first run it will actually produce an APK named for the `debug` configuration like this `com.andrewandderek.podcastutilities.sideload.debug-Signed.apk`. Not sure if this is an artefact of having the IDE running at the same time but running it for a second time seems to fix the issue. 

1. In VS Select Project Properties for `PodcastUtilities` -> Android Manifest enter the correct VersionName and VersionNumber
1. Open a developer command prompt for VS2022
1. Goto `Android\PodcastUtilities`
1. Run `BuildReleaseAPK.bat`
1. The `apk` will be copied to `Android\Support\CurrentBuild` and will be called `com.andrewandderek.podcastutilities-Signed.apk`
1. Goto `Android\Support\CurrentBuild`
1. Connect a test device
1. Run `InstallReleaseApk.bat`

##### Publishing a release APK

1. Copy the `com.andrewandderek.podcastutilities.sideload-Signed.apk` to `Android\Support\_PreBuiltPackages`
1. Edit `Android\Support\_PreBuiltPackages\README.md` with the new release details
1. Edit `Android\Support\_PreBuiltPackages\release.xml` with the new release details
1. Push your changes to master and label the commit in the style "v1.0.3(4)" ensure the tag is pushed by using `git push origin "1.0.3(4)"`
1. Upload the APK to the Amazon App Store

https://developer.amazon.com/apps-and-games/console/apps/list.html

##### Notes on installing release builds

You cannot upgrade from an AAB to an APK install, or visa versa, as they are signed with different keys. The AAB is signed by Google and the APK is signed by the developer.

Sometimes when running `BuildReleaseAPK.bat` it will generate a file called `com.andrewandderek.podcastutilities.debug-Signed.aab`, and the copy operation will fail, if this happens just run the script again.

##### Installing on Android Subsystem for Windows

On Windows 11 you will need to enable `Virtual Machine Platform` in `Windows Features`. Then visit the Microsoft Store and install `Amazon App Store`.

See https://support.microsoft.com/en-gb/windows/install-mobile-apps-and-the-amazon-appstore-on-windows-f8d0abb5-44ad-47d8-b9fb-ad6b1459ff6c

And https://developer.amazon.com/apps-and-games/blogs/2023/07/debug-android-apps-for-windows-11

And ms-windows-store://pdp/?productid=9NJHK44TTKSX

You need to sign in with your Amazon account

To start the subsystem either run an App or Files and then 

```
adb connect localhost:58526
```

Then you can use ADB to install or select Android Subsystem for Windows from the list of devices in Android Studio or Visual Studio

If you want to download podcasts to the Windows file system then you will need to change the source root to be something like this

```
<sourceRoot>/storage/emulated/0/Windows/Podcasts</sourceRoot>
```


