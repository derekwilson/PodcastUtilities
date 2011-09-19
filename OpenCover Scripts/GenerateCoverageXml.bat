call SetProgramFilesEnv.bat
if "%ProgramFilesPath%" == "" exit 1

set opencovercmd="%ProgramFilesPath%\OpenCover\OpenCover.Console.exe"

set nunitcmd="%ProgramFilesPath%\NUnit 2.5.10\bin\net-2.0\nunit-console-x86.exe"

%opencovercmd% -register:user -target:%nunitcmd% -targetargs:"/noshadow PodcastUtilities.nunit" -targetdir:"." -output:coverage.xml -filter:"+[PodcastUtilities.Common]* +[PodcastUtilities.Presentation]* -[PodcastUtilities.Common]PodcastUtilities.Common.Platform* -[PodcastUtilities.Common]PodcastUtilities.Common.Exceptions*"


