call ".\PartCover Scripts\GenerateCoverageXml.bat"

rem this is a generator available from http://www.palmmedia.de/Net/ReportGenerator
.\Tools\ReportGenerator\ReportGenerator.exe coverage.xml .\Coverage
start .\coverage\index.htm
pause
