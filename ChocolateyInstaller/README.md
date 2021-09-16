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

Update the `build.ps1` to reference the most up to date ZIP file

Goto folder `ChocolateyInstaller\podcastutilities-core` and run `build.ps1` in powershell

The package `podcastutilities-core.3.0.0.0.nupkg` will be created

### Install package

#### Locally

Get the package `podcastutilities-core.3.0.0.0.nupkg` in the current folder

```
choco install "podcastutilities-core" -s ".\" 
```

#### From worldolio web site

```
Invoke-WebRequest -Uri "https://worldolio.azurewebsites.net/Download/worldolio.2.0.0.0.nupkg.zip" -OutFile ".\worldolio.2.0.0.0.nupkg"
choco install "Worldolio" -s ".\" 
```

### Uninstall package

```
choco uninstall "podcastutilities-core"
```

### Publish package

(set you API key)

then

```
choco push podcastutilities-core.3.0.0.0.nupkg -s https://push.chocolatey.org/
```
