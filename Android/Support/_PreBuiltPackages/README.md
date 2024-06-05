# PodcastUtilities Release Archive
This is the release archive for PodcastUtilities. If you cannot get an APK from google you can side-load it from here

## APKs

| Version | Date        | MinSDK           | TargetSDK
| ------- | ----------- | ---------------- | ---------------
| 2.5.0   | 5 Jun 2024  | 21 (Android 5)   | 33 (Android 13)
| 2.4.0   | 20 May 2024 | 21 (Android 5)   | 33 (Android 13)
| 2.3.0   | 17 May 2024 | 21 (Android 5)   | 33 (Android 13)
| 2.2.0   | 16 Jan 2024 | 21 (Android 5)   | 33 (Android 13)
| 2.1.0   | 11 Jan 2024 | 21 (Android 5)   | 33 (Android 13)
| 2.0.0   | 28 Dec 2023 | 21 (Android 5)   | 33 (Android 13)
| 1.7.0   | 27 Nov 2023 | 21 (Android 5)   | 33 (Android 13)
| 1.6.0   | 17 Nov 2023 | 21 (Android 5)   | 33 (Android 13)
| 1.5.0   | 1 Nov 2023  | 21 (Android 5)   | 33 (Android 13)
| 1.4.0   | 26 Oct 2023 | 21 (Android 5)   | 33 (Android 13)
| 1.3.0   | 10 May 2023 | 21 (Android 5)   | 33 (Android 13)
| 1.2.0   | 21 Nov 2022 | 21 (Android 5)   | 33 (Android 13)
| 1.1.0   | 20 Oct 2022 | 21 (Android 5)   | 33 (Android 13)
| 1.0.3   | 1 Jun 2022  | 19 (Android 4.4) | 31 (Android 12)
| 1.0.2   | 14 Apr 2022 | 19 (Android 4.4) | 31 (Android 12)
| 1.0.1   | 7 Apr 2022  | 19 (Android 4.4) | 31 (Android 12)
| 1.0.0   | 7 Apr 2022  | 19 (Android 4.4) | 31 (Android 12)

## Notes

Major changes for each version

## v2.5.0 (17)
- Removed dependency on AppCenter as it is being retired

## v2.4.0 (16)
- Fixed sharing Feed RSS URL via WhatsApp
- When adding a podcast feed remove illegal chars from the folder name when its populated from testing the feed

## v2.3.0 (15)
- Updated `PodcastUtilities.Common.Dll` to v3.2.0.1
- Reworked the UI for adding a new feed to lookup the URL from the clipboard and the title from the feed
- Updated the main feed list display to be more useful
- Added ability to share a podcast RSS feed URL
- Added global configuration of number of concurrent downloads and the retry time when writing to state
- Display an error alert if there is an error testing the feed or finding episodes to download
- Handling of "." in episode titles that are used as filenames is improved
- FIX: Issue with filenames containing "|" and "*"

## v2.2.0 (14)
- Added delete button to edit feed page
- FIX: Issues with logging errors when finding episodes
- FIX: Issues with SSL handshake errors when finding episodes
- FIX: ESC key on feed defaults page

## v2.1.0 (13)
- Updated help text
- Added playlist config to global values screen
- Added diagnostic config to global values screen
- Added ability to test a feed from the edit feed screen
- FIX: scrolling issue in the config screens on Android versions before v10
- FIX: ESC key on config screens

## v2.0.0 (12)
- Added ability to modify the configuration from within the app
- Added ability to reset the configuration
- Updated help text

## v1.7.0 (11)
- Added keyboard support
- Updated help text
- Made available for Windows Subsystem for Android

## v1.6.0 (10)
- FIX: scrolling issue in Android versions before v10

## v1.5.0 (9)
- Accessibility fixes: Text scaling on toolbar and colour contrast

## v1.4.0 (8)
- Added support for dark mode
- Added shortcut buttons for Purge and Download
- Fixed permissions issues in Android 13
- FIX: Generate playlist displaying multiple status messages
- FIX: Issues with SSL handshake errors on some Samsung devices when downloading episodes

## v1.3.0 (7)
- Added ability to share the current control file off the device
- Updated the UI to display the number of items found while scanning

## v1.2.0 (6)
- Updated `PodcastUtilities.Common.Dll` to v3.1.0.0
- When finding episodes they are now listed sorted from oldest to newest (rather than rely on the order in the feed)
- Added support for `maximumNumberOfDownloadedItems` to throttle downloads
- FIX: `Latest` download strategy would download the latest episode not in the cache, it now does nothing if the latest episode is in the cache 
- FIX: Back button on the toolbar for the logging display now works correctly

## v1.1.0 (5)
- First version available for Kindle Fire devices
- switched from Crashlytics/GoogleAnalytics to AppCenter also removed all references to GooglePlayServices
- added ability to download a specific feed
- MinSDK moved to 21 as required by AppCenter SDK

## v1.0.3 (4)
- First version available through Amazon App Store
- Changed package name as Google blacklisted the old one (you will need to uninstall any previous version)
- Include the folder name in the list of files to purge
- UI Fixes: colours, FABs and settings menu

## v1.0.2 (3)
- Fixed crash on Licenses and Logs activity
- Fixed issue with pubdate_folder_title_url naming style
- Sort playlists and exclude single quotes from filenames
- Added help text

## v1.0.1 (2) - AAB release through Google Play
- Fixed permissions issue with Android 10/11

## v1.0.0 (1) - AAB release through Google Play
- Initial internal test release.




