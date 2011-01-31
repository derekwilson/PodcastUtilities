"C:\Program Files\PartCover\PartCover .NET 4.0\partcover.exe" --settings PodcastUtilities.Common.Tests.PartcoverSettings.xml --output coverage.xml
rem this is a generator available from http://www.palmmedia.de/Net/ReportGenerator
"C:\Program Files\ReportGenerator\ReportGenerator.exe" coverage.xml .\Coverage
start .\coverage\index.htm
