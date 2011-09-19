call SetProgramFilesEnv.bat
if "%ProgramFilesPath%" == "" exit 1

set partcovercmd="%ProgramFilesPath%\PartCover\PartCover .NET 4.0\partcover.exe"

%partcovercmd% --target ".\PartCover Scripts\runnunit.bat" --target-work-dir ".\PodcastUtilities.Common.Tests\bin\Debug" --output coverage.xml --include "[PodcastUtilities.Common]*" --exclude "[PodcastUtilities.Common]PodcastUtilities.Common.Platform*" --exclude "[PodcastUtilities.Common]PodcastUtilities.Common.Exceptions*"


