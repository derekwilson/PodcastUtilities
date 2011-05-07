if not "%PROGRAMFILES(x86)%" == "" goto win64
if not "%ProgramFiles%" == "" goto win32
echo Cannot find program files environment variable
pause
goto end

:win32
set nunitcmd="%ProgramFiles%\NUnit 2.5.7\bin\net-2.0\nunit-console-x86.exe"
goto nunit

:win64
set nunitcmd="%PROGRAMFILES(x86)%\NUnit 2.5.7\bin\net-2.0\nunit-console-x86.exe" 

:nunit
%nunitcmd% /noshadow PodcastUtilities.Common.Tests.dll

:end
