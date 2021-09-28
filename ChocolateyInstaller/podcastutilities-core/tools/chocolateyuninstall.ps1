$ErrorActionPreference = 'Stop';
$PackageName = $env:ChocolateyPackageName;
$StartMenu = "$([System.Environment]::GetFolderPath('CommonStartMenu'))\Programs"
$Desktop = [System.Environment]::GetFolderPath("Desktop");
$PrgmData = [System.Environment]::GetFolderPath("CommonApplicationData");

# we only need to do the things that the auto uninstall doesnt do
Uninstall-BinFile -Name downloadpodcasts-core
Uninstall-BinFile -Name generateplaylist-core
Uninstall-BinFile -Name purgepodcasts-core
Uninstall-BinFile -Name syncpodcasts-core

Remove-Item "$Desktop\PodcastUtilitiesCore.lnk" -ErrorAction SilentlyContinue -Force | Out-Null
Remove-Item "$StartMenu\PodcastUtilitiesCore.lnk" -ErrorAction SilentlyContinue -Force | Out-Null
