del PodcastUtilities\bin\Release\com.andrewandderek.podcastutilities*.apk
msbuild PodcastUtilities\PodcastUtilities.csproj /p:Configuration=Release /t:Clean;SignAndroidPackage /p:AndroidPackageFormat=apk
copy PodcastUtilities\bin\Release\com.andrewandderek.podcastutilities.sideload-Signed.apk ..\Support\CurrentBuild
pause