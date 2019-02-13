call SetProgramFilesEnv.bat
if "%ProgramFilesPath%" == "" exit 1

set opencovercmd="%ProgramFilesPath%\OpenCover\OpenCover.Console.exe"

set nunitcmd="%ProgramFilesPath%\NUnit 2.7.0\bin\nunit-console-x86.exe"

%opencovercmd% -register:user -target:%nunitcmd% -targetargs:"/noshadow /apartment:mta PodcastUtilities.nunit" -targetdir:"." -output:coverage.xml -filter:"+[PodcastUtilities.Common]* +[PodcastUtilities.Presentation]* -[PodcastUtilities.Common]PodcastUtilities.Common.Platform* -[PodcastUtilities.Common]PodcastUtilities.Common.Exceptions*"

rem this is a generator available from http://reportgenerator.codeplex.com/
.\Tools\ReportGenerator\net47\ReportGenerator.exe -reports:coverage.xml -targetdir:.\Coverage
rem pause
start .\coverage\index.htm
