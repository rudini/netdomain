@echo off
if exist "%ProgramFiles%\Microsoft Visual Studio 10.0\VC\vcvarsall.bat" call "%ProgramFiles%\Microsoft Visual Studio 9.0\VC\vcvarsall.bat"

..\Tools\nant-0.91-alpha2\bin\NAnt -buildfile:nant.build compile.release -l:netdomain-Build.log
IF ERRORLEVEL 1 GOTO Failed

echo "compilation completed."
GOTO End


:Failed
echo "Failed"


:End
pause