@echo off

REM -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
REM Compiles the files and builds the deployment packages
REM -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=

setlocal

CALL "Build Scripts\GetBuildPath.cmd"
IF "%MSBUILD%"=="" GOTO end

"%MSBUILD%" "Build Scripts\build.xml" /p:Configuration=Release;Environment=All

:end
pause
