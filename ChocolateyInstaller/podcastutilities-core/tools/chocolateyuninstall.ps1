$ErrorActionPreference = 'Stop';
$PackageName = $env:ChocolateyPackageName;
$StartMenu = "$([System.Environment]::GetFolderPath('CommonStartMenu'))\Programs"
$Desktop = [System.Environment]::GetFolderPath("Desktop");
$PrgmData = [System.Environment]::GetFolderPath("CommonApplicationData");
$installDir = Join-Path $PrgmData "podcastitilities-core"

# we only need to do the things that the auto uninstall doesnt do
Uninstall-BinFile -Name downloadpodcasts-core
Remove-Item –path "$installDir" –recurse -ErrorAction SilentlyContinue -Force
