call GenerateCoverageXml.bat
rem this is a generator available from http://www.palmmedia.de/Net/ReportGenerator
"C:\Program Files\PartCover\ReportGenerator\ReportGenerator.exe" coverage.xml .\Coverage
start .\coverage\index.htm
