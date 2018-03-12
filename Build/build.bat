cd /d %~dp0\..\
echo off

set BUILD=Build
set ERROR=%BUILD%\@error
set PROJECT=NKit

set OUTPUT=Output\Release
set OUTPUT_x86=%OUTPUT%\AnyCPU\%PROJECT%
set VER_TARGET=%OUTPUT_x86%\NKit.exe
set ZIP=%BUILD%\zip.vbs
set GV=%BUILD%\get-ver.vbs

set DOTNETVER=v4.0.30319

del /Q %ERROR%

rmdir /S /Q "%OUTPUT_x86%"

rem if "%PROCESSOR_ARCHITECTURE%" NEQ "x86" (
rem 	set MB=%windir%\microsoft.net\framework64\%DOTNETVER%\msbuild
rem ) else (
rem 	set MB=%windir%\microsoft.net\framework\%DOTNETVER%\msbuild
rem )
if not defined MB set MB=%ProgramFiles(x86)%\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\MSBuild.exe

echo build x86
"%MB%" Source\NKit.sln /p:DefineConstants="BUILD;%1" /p:Configuration=Release;Platform="Any CPU" /t:Rebuild /m /p:TargetFrameworkVersion=v4.7 /p:CodeAnalysisRuleSet=
set ERROR_X86=%ERRORLEVEL%

if not %ERROR_X86% == 0 echo "build error x86: %ERROR_X86%" >> "%ERROR%"

for /F "usebackq" %%s in (`cscript "%GV%" "%VER_TARGET%"`) do set EXEVER=%%s

if "%2" == "FULL" goto REMOVED

echo remove
echo remove *.pdb, *.xml
del /S /Q "%OUTPUT_x86%\*.pdb
del /S /Q "%OUTPUT_x86%\lib\*.xml"

:REMOVED

echo compression
cscript "%ZIP%" "%OUTPUT_x86%" "%OUTPUT%\NKit_%EXEVER%_AnyCPU.zip"

