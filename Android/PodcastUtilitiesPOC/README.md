## Building the app

Run the Developer Command Prompt

clean

```
msbuild /t:PackageForAndroid /p:Configuration=Release PodcastUtilitiesPOCSolution.sln
```

build release

```
msbuild /t:Install /p:Configuration=Release /p:AdbTarget=-e PodcastUtilitiesPOCSolution.sln
```

