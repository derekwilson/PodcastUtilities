call GenerateCoverageXml.bat

if "%ProgramFilesPath%" == "" exit 1

rem this is a generator available from http://www.palmmedia.de/Net/ReportGenerator
"%ProgramFilesPath%\PartCover\ReportGenerator\ReportGenerator.exe" coverage.xml .\Coverage
start .\coverage\index.htm
