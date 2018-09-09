cd /d %~dp0\..\
echo off

set BUILD=Build
set ERROR=%BUILD%\@error
set PROJECT=NKit

set OUTPUT=Output\Release
set OUTPUT_ANY=%OUTPUT%\AnyCPU\%PROJECT%
set VER_TARGET=%OUTPUT_ANY%\NKit.exe
set ZIP=%BUILD%\zip.vbs
set GV=%BUILD%\get-ver.vbs

set DOTNETVER=v4.0.30319

del /Q %ERROR%

rmdir /S /Q "%OUTPUT_ANY%"

rem if "%PROCESSOR_ARCHITECTURE%" NEQ "x86" (
rem 	set MB=%windir%\microsoft.net\framework64\%DOTNETVER%\msbuild
rem ) else (
rem 	set MB=%windir%\microsoft.net\framework\%DOTNETVER%\msbuild
rem )
if not defined MB set MB=%ProgramFiles(x86)%\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\MSBuild.exe

echo build Any CPU
"%MB%" Source\NKit.sln /p:DefineConstants="BUILD;BUILD_PLATFORM_ANYCPU;%1" /p:Configuration=Release;Platform="Any CPU" /t:Rebuild /m /p:TargetFrameworkVersion=v4.6 /p:CodeAnalysisRuleSet=
set ERROR_ANY=%ERRORLEVEL%

if not %ERROR_ANY% == 0 echo "build error Any CPU: %ERROR_ANY%" >> "%ERROR%"

for /F "usebackq" %%s in (`cscript "%GV%" "%VER_TARGET%"`) do set EXEVER=%%s

del /Q "%OUTPUT_ANY%\@NKit.*.user.config

if "%2" == "FULL" goto REMOVED

echo remove
echo remove *.pdb, *.xml, @NKit.*.user.config
del /S /Q "%OUTPUT_ANY%\*.pdb
del /S /Q "%OUTPUT_ANY%\lib\*.xml"

:REMOVED

echo compression
cscript "%ZIP%" "%OUTPUT_ANY%" "%OUTPUT%\NKit_%EXEVER%_AnyCPU.zip"

echo output version
echo %EXEVER% > %OUTPUT%\version.txt

