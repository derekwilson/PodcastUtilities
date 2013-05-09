@echo off

REM -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
REM Locates the MSBuild tool
REM -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=

set MSBUILD=
set MSBUILD2005=%SystemRoot%\Microsoft.NET\Framework\v2.0.50727\msbuild.exe
set MSBUILD2008=%SystemRoot%\Microsoft.NET\Framework\v3.5\msbuild.exe
set MSBUILD2010=%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe

REM To change the default version of MSBuild EXE, change the order of the following parameters.
REM eg to make MSBuild 2008 the default put it first in the list
call :CheckLocation "%MSBUILD2010%" "%MSBUILD2008%" "%MSBUILD2005%"

goto :end


:CheckLocation

if "%~1"=="" goto :eof

if exist "%~1" (
  set MSBUILD=%~1
  ECHO Using %~1
  goto :eof
)
shift
goto :CheckLocation


:end

if "%MSBUILD%"=="" echo Could not find the location of the MSBuild installation
