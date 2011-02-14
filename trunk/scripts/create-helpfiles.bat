@echo off
if exist "%ProgramFiles%\Microsoft Visual Studio 10.0\VC\vcvarsall.bat" call "%ProgramFiles%\Microsoft Visual Studio 9.0\VC\vcvarsall.bat"

..\Tools\NAnt\NAnt -buildfile:nant.build create.helpfiles 
pause
