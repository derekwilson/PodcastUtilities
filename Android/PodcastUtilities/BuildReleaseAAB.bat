del PodcastUtilities\bin\Release\com.andrewandderek.podcastutilities*.aab
msbuild PodcastUtilities\PodcastUtilities.csproj /p:Configuration=Release /t:Clean;SignAndroidPackage /p:AndroidPackageFormat=aab
copy PodcastUtilities\bin\Release\com.andrewandderek.podcastutilities-Signed.aab ..\Support\CurrentBuild
pause