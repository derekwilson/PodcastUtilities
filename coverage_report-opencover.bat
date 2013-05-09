call ".\OpenCover Scripts\GenerateCoverageXmlv3.bat"

rem this is a generator available from http://reportgenerator.codeplex.com/
.\Tools\ReportGenerator\ReportGenerator.exe coverage.xml .\Coverage
rem pause
start .\coverage\index.htm
