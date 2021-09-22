$packageName = 'podcastutilities-core'
$StartMenu = "$([System.Environment]::GetFolderPath('CommonStartMenu'))\Programs"
$Desktop = [System.Environment]::GetFolderPath("Desktop");
$PrgmFiles = [System.Environment]::GetFolderPath("ProgramFiles");
$PrgmData = [System.Environment]::GetFolderPath("CommonApplicationData");
$UserPrgmData = [System.Environment]::GetFolderPath("LocalApplicationData");
$toolsDir = "$(Split-Path -parent $MyInvocation.MyCommand.Definition)"
$filePath = "$(Split-Path -Parent $MyInvocation.MyCommand.Definition)\\podcastitilities-core.zip"
$defaultDotnetRuntimePath = "C:\Program Files\dotnet\dotnet.exe"

$ErrorActionPreference = 'Stop'; # stop on all errors

Get-ChocolateyUnzip -FileFullPath "$filePath" -Destination $toolsDir
# we could remove the zip for total neatness
#remove-item "$filePath"

if (!(Test-Path $defaultDotnetRuntimePath))
{
    Write-Host -ForegroundColor Red "File not found: $defaultDotnetRuntimePath"
    Write-Host "The package depends on the .NET Core Runtime (dotnet.exe) which was not found."
    Write-Host "Please install the latest version of the .NET Core Runtime to use this package."
    exit 1
}

# manually shim as these are .NET Core DLLs
Install-Binfile -Name downloadpodcasts-core -Path "$defaultDotnetRuntimePath" -Command Join-Path $toolsDir "DownloadPodcasts.dll"

$target = Join-Path $toolsDir "Podcast Utilities User Guide.docx"
# create a desktop shortcut
Install-ChocolateyShortcut -shortcutFilePath "$Desktop\PodcastUtilities.lnk" -targetPath "$target" -workDirectory "$toolsDir" -arguments "" -description "PodcastUtilities"
# create a start menu shortcut
Install-ChocolateyShortcut -shortcutFilePath "$StartMenu\PodcastUtilities.lnk" -targetPath "$target" -workDirectory "$toolsDir" -arguments "" -description "PodcastUtilities"

