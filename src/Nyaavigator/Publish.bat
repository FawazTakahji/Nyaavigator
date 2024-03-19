@echo off

cls
for /f %%a in ('echo prompt $E^| cmd') do set "ESC=%%a"
set "NC=%ESC%[0m"
set "RED=%ESC%[31m"
set "BLUE=%ESC%[34m"

:platform
set /p "platformselect=Choose platform (1 - Windows, 2 - Linux): "
if "%platformselect%"=="1" (
  set "rid=win-x64"
  set "framework=net8.0-windows"
  cls
) else if "%platformselect%"=="2" (
  set "rid=linux-x64"
  set "framework=net8.0"
  cls
) else (
  cls
  echo %RED%Invalid platform selection. Please choose 1 or 2.%NC%
  echo:
  goto :platform
)

:configuration
set /p "config=Choose configuration (1 - Release, 2 - Portable): "
if "%config%"=="1" (
  set "config=Release"
  cls
) else if "%config%"=="2" (
  set "config=Portable"
  cls
) else (
  cls
  echo %RED%Invalid configuration selection. Please choose 1 or 2.%NC%
  echo:
  goto :configuration
)

echo Executing: dotnet publish Nyaavigator.csproj -c %BLUE%%config%%NC% -r %BLUE%%rid%%NC% -f %BLUE%%framework%%NC% -o bin/Publish/%BLUE%%rid%%NC%/%BLUE%%config%%NC% --self-contained -p:PublishSingleFile=true
echo:
pause
cls
dotnet publish Nyaavigator.csproj -c %config% -r %rid% -f %framework% -o bin/Publish/%rid%/%config% --self-contained -p:PublishSingleFile=true
echo:
start "" bin\Publish\%rid%\%config%

set /p "choice=Restart (Y)/N? "
if "%choice%"=="n" (
  goto :eof
) else if "%choice%"=="N" (
  goto :eof
) else (
  cls
  goto :platform
)
