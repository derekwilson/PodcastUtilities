del PodcastUtilities\bin\Release\net9.0-android\com.andrewandderek.podcastutilities*.aab
PowerShell -ExecutionPolicy Unrestricted -File ".\Update-PackageName.ps1" ".\PodcastUtilities\\" "Release"
dotnet publish -c Release -p:AndroidPackageFormats=aab
copy PodcastUtilities\bin\Release\net9.0-android\com.andrewandderek.podcastutilities.sideload-Signed.aab ..\Support\CurrentBuild
pause