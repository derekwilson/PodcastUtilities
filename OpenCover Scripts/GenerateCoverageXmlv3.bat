call SetProgramFilesEnv.bat
if "%ProgramFilesPath%" == "" exit 1

set opencovercmd="%LocalAppData%\Apps\OpenCover\OpenCover.Console.exe"

set nunitcmd="%ProgramFilesPath%\NUnit 2.6\bin\nunit-console-x86.exe"

%opencovercmd% -register:user -target:%nunitcmd% -targetargs:"/noshadow /apartment:mta PodcastUtilities.nunit" -targetdir:"." -output:coverage.xml -filter:"+[PodcastUtilities.Common]* +[PodcastUtilities.Presentation]* -[PodcastUtilities.Common]PodcastUtilities.Common.Platform* -[PodcastUtilities.Common]PodcastUtilities.Common.Exceptions*"


