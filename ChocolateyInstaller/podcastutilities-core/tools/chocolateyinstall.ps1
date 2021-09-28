$packageName = 'podcastutilities-core'
$StartMenu = "$([System.Environment]::GetFolderPath('CommonStartMenu'))\Programs"
$Desktop = [System.Environment]::GetFolderPath("Desktop");
$PrgmFiles = [System.Environment]::GetFolderPath("ProgramFiles");
$PrgmData = [System.Environment]::GetFolderPath("CommonApplicationData");
$UserPrgmData = [System.Environment]::GetFolderPath("LocalApplicationData");
$toolsDir = "$(Split-Path -parent $MyInvocation.MyCommand.Definition)"
$filePath = "$(Split-Path -Parent $MyInvocation.MyCommand.Definition)\podcastutilities-core3.0.0.2.zip"
$defaultDotnetRuntimePath = "C:\Program Files\dotnet\dotnet.exe"

$ErrorActionPreference = 'Stop'; # stop on all errors

Get-ChocolateyUnzip -FileFullPath "$filePath" -Destination $toolsDir
# we could remove the zip for total neatness
#remove-item "$filePath"

if ((Test-Path $defaultDotnetRuntimePath))
{
    # manually shim as these are .NET Core DLLs
    Install-Binfile -Name downloadpodcasts-core -Path "$defaultDotnetRuntimePath" -Command "$(Join-Path $toolsDir DownloadPodcasts.dll)"
    Install-Binfile -Name generateplaylist-core -Path "$defaultDotnetRuntimePath" -Command "$(Join-Path $toolsDir GeneratePlaylist.dll)"
    Install-Binfile -Name purgepodcasts-core -Path "$defaultDotnetRuntimePath" -Command "$(Join-Path $toolsDir PurgePodcasts.dll)"
    Install-Binfile -Name syncpodcasts-core -Path "$defaultDotnetRuntimePath" -Command "$(Join-Path $toolsDir SyncPodcasts.dll)"
}
else 
{
    Write-Host -ForegroundColor Red "File not found: $defaultDotnetRuntimePath"
    Write-Host "The DLL shims depend on the .NET Core Runtime (dotnet.exe) which was not found."
    Write-Host "The package files have been installed, if you want to shims to be created then"
    Write-Host "please install the latest version of the .NET Core Runtime and reinstall this package."
}

$target = Join-Path $toolsDir "Podcast Utilities User Guide.docx"
# create a desktop shortcut
Install-ChocolateyShortcut -shortcutFilePath "$Desktop\PodcastUtilitiesCore.lnk" -targetPath "$target" -workDirectory "$toolsDir" -arguments "" -description "PodcastUtilitiesCore"
# create a start menu shortcut
Install-ChocolateyShortcut -shortcutFilePath "$StartMenu\PodcastUtilitiesCore.lnk" -targetPath "$target" -workDirectory "$toolsDir" -arguments "" -description "PodcastUtilitiesCore"

