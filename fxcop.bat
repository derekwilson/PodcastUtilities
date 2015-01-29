call SetProgramFilesEnv.bat
if "%ProgramFilesPath%" == "" exit 1

rem set fxcopcmd="%ProgramFilesPath%\Microsoft Fxcop 10.0\FxCopCmd.exe"
set fxcopcmd="%ProgramFilesPath%\Microsoft Fxcop 10.0\FxCop.exe"

%fxcopcmd% /p:pucastutilities.fxcop /dictionary:FxCopDictionary.xml