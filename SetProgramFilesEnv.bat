set ProgramFilesPath=

if not "%PROGRAMFILES(x86)%" == "" goto win64
if not "%ProgramFiles%" == "" goto win32
echo Cannot find program files environment variable
pause
goto end

:win32
set ProgramFilesPath=%ProgramFiles%
goto end

:win64
set ProgramFilesPath=%PROGRAMFILES(x86)%

:end
