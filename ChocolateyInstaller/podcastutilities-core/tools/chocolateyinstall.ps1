$packageName = 'podcastutilities-core'
$StartMenu = "$([System.Environment]::GetFolderPath('CommonStartMenu'))\Programs"
$Desktop = [System.Environment]::GetFolderPath("Desktop");
$PrgmFiles = [System.Environment]::GetFolderPath("ProgramFiles");
$PrgmData = [System.Environment]::GetFolderPath("CommonApplicationData");
$UserPrgmData = [System.Environment]::GetFolderPath("LocalApplicationData");
$toolsDir = "$(Split-Path -parent $MyInvocation.MyCommand.Definition)"
$filePath = "$(Split-Path -Parent $MyInvocation.MyCommand.Definition)\\podcastitilities-core.zip"
$installDir = Join-Path $PrgmData "podcastitilities-core"
$defaultDotnetRuntimePath = "C:\Program Files\dotnet\dotnet.exe"

$ErrorActionPreference = 'Stop'; # stop on all errors

# unzip the files to a foler that dies not require admin access
Get-ChocolateyUnzip -FileFullPath "$filePath" -Destination $installDir
# we could remove the zip for total neatness
#remove-item "$filePath"

if (!(Test-Path $defaultDotnetRuntimePath))
{
    Write-Host -ForegroundColor Red "File not found: $defaultDotnetRuntimePath"
    Write-Host "The package depends on the .NET Core Runtime (dotnet.exe) which was not found."
    Write-Host "Please install the latest version of the .NET Core Runtime to use this package."
    exit 1
}

# manually shim as we are not in the default folder
Install-Binfile -Name downloadpodcasts-core -Path "$defaultDotnetRuntimePath" -Command Join-Path $installDir "DownloadPodcasts.dll"