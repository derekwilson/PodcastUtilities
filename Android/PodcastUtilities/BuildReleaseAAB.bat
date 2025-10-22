del PodcastUtilities\bin\Release\net9.0-android\com.andrewandderek.podcastutilities*.aab
dotnet clean --configuration Debug
dotnet clean --configuration Release
dotnet publish -c Release -p:AndroidPackageFormats=aab
copy PodcastUtilities\bin\Release\net9.0-android\com.andrewandderek.podcastutilities.sideload-Signed.aab ..\Support\CurrentBuild
pause