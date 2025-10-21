del PodcastUtilities\bin\Release\net9.0-android\com.andrewandderek.podcastutilities*.apk
PowerShell -ExecutionPolicy Unrestricted -File ".\Update-PackageName.ps1" ".\PodcastUtilities\\" "Release"
dotnet publish -c Release -p:AndroidPackageFormats=apk
copy PodcastUtilities\bin\Release\net9.0-android\com.andrewandderek.podcastutilities.sideload-Signed.apk ..\Support\CurrentBuild
pause