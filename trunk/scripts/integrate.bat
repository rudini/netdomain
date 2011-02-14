@echo off
if exist "%ProgramFiles%\Microsoft Visual Studio 10.0\VC\vcvarsall.bat" call "%ProgramFiles%\Microsoft Visual Studio 9.0\VC\vcvarsall.bat"
set resultDir=..\results
if not exist %resultDir% mkdir %resultDir%


..\Tools\NAnt\NAnt -buildfile:nant.build -l:%resultDir%\FinancePlus-Nant-Build.log integrate 
IF ERRORLEVEL 1 GOTO Failed

echo "compilation and unit testing completed. Log file and unit-tests results are stored in %resultDir%"
GOTO End


:Failed
echo "Failed"


:End
pause
