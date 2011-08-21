call ".\OpenCover Scripts\GenerateCoverageXml.bat"

rem this is a generator available from http://reportgenerator.codeplex.com/
set reportgenerator="D:\Tools\ReportGenerator_1.1.1.0\bin\ReportGenerator.exe"

%reportgenerator% coverage.xml .\Coverage
start .\coverage\index.htm
