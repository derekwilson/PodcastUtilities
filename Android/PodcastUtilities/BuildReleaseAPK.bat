del PodcastUtilities\bin\Release\net9.0-android\com.andrewandderek.podcastutilities*.apk
dotnet clean --configuration Debug
dotnet clean --configuration Release
dotnet publish -c Release -p:AndroidPackageFormats=apk
copy PodcastUtilities\bin\Release\net9.0-android\com.andrewandderek.podcastutilities.sideload-Signed.apk ..\Support\CurrentBuild
pause