if not "%PROGRAMFILES(x86)%" == "" goto win64
if not "%ProgramFiles%" == "" goto win32
echo Cannot find program files environment variable
pause
goto end

:win32
set partcovercmd="%ProgramFiles%\PartCover\PartCover .NET 4.0\partcover.exe"
goto partcover

:win64
set partcovercmd="%PROGRAMFILES(x86)%\PartCover\PartCover .NET 4.0\partcover.exe" 

:partcover
%partcovercmd% --target "runnunit.bat" --target-work-dir ".\PodcastUtilities.Common.Tests\bin\Debug" --output coverage.xml --include "[PodcastUtilities.Common]*"

:end

