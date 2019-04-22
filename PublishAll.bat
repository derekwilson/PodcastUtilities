dotnet publish -c Release -f netcoreapp2.1 .\PodcastUtilities.Integration.Tests.Multiplatform\PodcastUtilities.Integration.Tests.Multiplatform.csproj
dotnet publish -c Release -f netcoreapp2.1 .\DownloadPodcasts.Multiplatform\DownloadPodcasts.Multiplatform.csproj
dotnet publish -c Release -f netcoreapp2.1 .\GeneratePlaylist.Multiplatform\GeneratePlaylist.Multiplatform.csproj
dotnet publish -c Release -f netcoreapp2.1 .\PurgePodcasts.Multiplatform\PurgePodcasts.Multiplatform.csproj
dotnet publish -c Release -f netcoreapp2.1 .\SyncPodcasts.Multiplatform\SyncPodcasts.Multiplatform.csproj
pause
