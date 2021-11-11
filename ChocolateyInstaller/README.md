### Install chocolatey

https://docs.chocolatey.org/en-us/choco/setup

CMD Install

```
@"%SystemRoot%\System32\WindowsPowerShell\v1.0\powershell.exe" -NoProfile -InputFormat None -ExecutionPolicy Bypass -Command "[System.Net.ServicePointManager]::SecurityProtocol = 3072; iex ((New-Object System.Net.WebClient).DownloadString('https://community.chocolatey.org/install.ps1'))" && SET "PATH=%PATH%;%ALLUSERSPROFILE%\chocolatey\bin"
```

Powershell

```
Set-ExecutionPolicy Bypass -Scope Process -Force; [System.Net.ServicePointManager]::SecurityProtocol = [System.Net.ServicePointManager]::SecurityProtocol -bor 3072; iex ((New-Object System.Net.WebClient).DownloadString('https://community.chocolatey.org/install.ps1'))
```

Test

https://blog.chocolatey.org/2020/01/remove-support-for-old-tls-versions/

```
[System.Net.ServicePointManager]::SecurityProtocol = 3072
[Enum]::GetNames([Net.SecurityProtocolType])
```

Offiline install

https://docs.chocolatey.org/en-us/choco/setup#completely-offline-install

Download package

https://community.chocolatey.org/packages/chocolatey/0.10.15

Unzip

Poweshell as Admin

```
Set-ExecutionPolicy Bypass -Scope Process
cd .\tools
& .\chocolateyInstall.ps1
choco upgrade chocolatey -y
```

### Create package

Build all the assemblies and create a current build ZIP in  `_PreBuiltPackages`

Goto folder `ChocolateyInstaller\podcastutilities-core` or `ChocolateyInstaller\podcastutilities` edit the `podcastutilities.nuspec` to have the correct version number, also alter the version number in `build.ps1` and `chocolateyinstall.ps1`

Run `build.ps1` in powershell

Running `.\build.ps1` it will create a zip file from the `CurrentBuild` folder and then copy it to the correct place and generate the verification hash

The package `podcastutilities-core.3.x.y.z.nupkg` or `podcastutilities.3.x.y.z.nupkg` will be created

### Install package

#### Locally

Get the package `podcastutilities-core.3.x.y.z.nupkg` or `podcastutilities.3.x.y.z.nupkg` in the current folder

```
choco install "podcastutilities-core" -s ".\" 
```
or
```
choco install "podcastutilities" -s ".\" 
```

### Uninstall package

```
choco uninstall "podcastutilities-core"
```
or
```
choco uninstall "podcastutilities"
```

### Publish package

(set you API key)

then

```
choco push podcastutilities-core.3.0.0.2.nupkg -s https://push.chocolatey.org/
```
or
```
choco push podcastutilities.3.0.0.2.nupkg -s https://push.chocolatey.org/
```
