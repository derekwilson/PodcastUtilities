$packageName = 'podcastutilities'
$StartMenu = "$([System.Environment]::GetFolderPath('CommonStartMenu'))\Programs"
$Desktop = [System.Environment]::GetFolderPath("Desktop");
$PrgmFiles = [System.Environment]::GetFolderPath("ProgramFiles");
$PrgmData = [System.Environment]::GetFolderPath("CommonApplicationData");
$UserPrgmData = [System.Environment]::GetFolderPath("LocalApplicationData");
$toolsDir = "$(Split-Path -parent $MyInvocation.MyCommand.Definition)"
$filePath = "$(Split-Path -Parent $MyInvocation.MyCommand.Definition)\podcastutilities3.0.2.7.zip"

$ErrorActionPreference = 'Stop'; # stop on all errors

Get-ChocolateyUnzip -FileFullPath "$filePath" -Destination $toolsDir
# we could remove the zip for total neatness
#remove-item "$filePath"

$target = Join-Path $toolsDir "Podcast Utilities User Guide.docx"
# create a desktop shortcut
Install-ChocolateyShortcut -shortcutFilePath "$Desktop\PodcastUtilities.lnk" -targetPath "$target" -workDirectory "$toolsDir" -arguments "" -description "PodcastUtilities"
# create a start menu shortcut
Install-ChocolateyShortcut -shortcutFilePath "$StartMenu\PodcastUtilities.lnk" -targetPath "$target" -workDirectory "$toolsDir" -arguments "" -description "PodcastUtilities"

