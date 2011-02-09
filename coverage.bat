call GenerateCoverageXml.bat

if not "%PROGRAMFILES(x86)%" == "" goto win64
if not "%ProgramFiles%" == "" goto win32
echo Cannot find program files environment variable
pause
goto end

:win32
set partcoverbrowsercmd="%ProgramFiles%\PartCover\PartCover .NET 4.0\partcover.browser.exe"
goto partcover

:win64
set partcoverbrowsercmd="%PROGRAMFILES(x86)%\PartCover\PartCover .NET 4.0\partcover.browser.exe" 

:partcover
%partcoverbrowsercmd% --report coverage.xml

:end
