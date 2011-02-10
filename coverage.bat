call GenerateCoverageXml.bat

if "%ProgramFilesPath%" == "" exit 1

set partcoverbrowsercmd="%ProgramFilesPath%\PartCover\PartCover .NET 4.0\partcover.browser.exe"

%partcoverbrowsercmd% --report coverage.xml
