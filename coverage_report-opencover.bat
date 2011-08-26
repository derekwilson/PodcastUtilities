call ".\OpenCover Scripts\GenerateCoverageXml.bat"

rem this is a generator available from http://reportgenerator.codeplex.com/
.\Tools\ReportGenerator\ReportGenerator.exe coverage.xml .\Coverage
start .\coverage\index.htm
