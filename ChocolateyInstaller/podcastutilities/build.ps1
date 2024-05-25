# Script builds a Chocolatey Package and tests it locally
# 
#  Assumes: ../_PreBuiltPackages folder has the files to ship
#   Builds: ChocolateyInstall.ps1 file with download URL and sha256 embedded

Set-Location "$PSScriptRoot" 

$Version = "3.2.0.1"
$ZipFile = "podcastutilities$Version.zip"
$SrcZipPath = Join-Path ".\files\" $ZipFile
$ZipPath = Join-Path ".\tools\" $ZipFile

If(!(test-path ".\files\"))
{
      New-Item -ItemType Directory -Force -Path ".\files\"
}
remove-item $SrcZipPath -ErrorAction SilentlyContinue
compress-archive ..\..\CurrentBuild\net35\*.* $SrcZipPath
remove-item ".\tools\*.zip" -ErrorAction SilentlyContinue
Copy-Item $SrcZipPath -Destination  $ZipPath

$sha = get-filehash -path $ZipPath -Algorithm SHA256  | select -ExpandProperty "Hash"
write-host $sha

$filetext = @"
`VERIFICATION
`Verification is intended to assist the Chocolatey moderators and community
`in verifying that this package's contents are trustworthy.
`
`The software is installed from the archive embedded in this nuppkg: $ZipFile
`It can also be obtained from <https://github.com/derekwilson/PodcastUtilities/tree/master/ChocolateyInstaller/podcastutilities/files>
`
`You can use one of the following methods to obtain the checksum
`  - Use powershell function 'Get-Filehash'
`    get-filehash $ZipFile
`  - Use chocolatey utility 'checksum.exe'
`    C:\ProgramData\chocolatey\tools\checksum.exe .\$ZipFile -t=sha256
`
`Compare the SHA256 Checksum Value: $sha
`
File 'LICENSE.txt' is obtained from <https://github.com/derekwilson/PodcastUtilities/blob/master/LICENSE.txt>
"@
out-file -filepath .\tools\Verification.txt -inputobject $filetext

Remove-Item *.nupkg

# Create .nupkg from .nuspec    
choco pack

choco uninstall "podcastutilities"

#choco install ""podcastutilities" -fd -y  -s ".\" 
choco install "podcastutilities" -y -s ".\" 
