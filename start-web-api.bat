if not exist "build_logs" mkdir "build_logs"
set SOLUTION1=FunGame.Core\FunGame.Core.sln
set SOLUTION2=FunGame.Server\FunGameServer.sln
set SOLUTION3=OshimaGameModule\OshimaGameModule.sln
set LOG_FILE=build_logs\build_log.txt
echo Build started at %date% %time% > %LOG_FILE%
echo ----------------------------------------------- >> %LOG_FILE%
echo Building FunGame.Core...
dotnet build %SOLUTION1% -c Debug >> %LOG_FILE% 2>&1
if %errorlevel% neq 0 (
    echo Failed to build FunGame.Core. See log for details.
    goto :error
)
echo FunGame.Core build succeeded.
echo Building FunGame.Server...
dotnet build %SOLUTION2% -c Debug >> %LOG_FILE% 2>&1
if %errorlevel% neq 0 (
    echo Failed to build FunGame.Server. See log for details.
    goto :error
)
echo FunGame.Server build succeeded.
echo Building OshimaGameModule...
dotnet build %SOLUTION3% -c Debug >> %LOG_FILE% 2>&1
if %errorlevel% neq 0 (
    echo Failed to build OshimaGameModule. See log for details.
    goto :error
)
echo OshimaGameModule build succeeded.
echo ----------------------------------------------- >> %LOG_FILE%
echo Build completed at %date% %time% >> %LOG_FILE%
echo All solutions built completely!
goto :end

:error
echo ----------------------------------------------- >> %LOG_FILE%
echo Build failed at %date% %time%. >> %LOG_FILE%
echo Check the log file for details: %LOG_FILE%

:end
set "coreDir=FunGame.Core\Library\SQLScript"
set "sourceDir=OshimaGameModule\bin\Debug\net10.0"
set "destDir=FunGame.Server\bin\Debug\net10.0"
set "destDir2=%destDir%\plugins\OshimaServer"
set "destDir3=%destDir%\plugins\OshimaWebAPI"
copy "%coreDir%\fungame.sql" "%destDir%" /y
copy "%coreDir%\fungame_sqlite.sql" "%destDir%" /y
if not exist "%destDir%\maps" mkdir "%destDir%\maps"
copy "%sourceDir%\OshimaCore.dll" "%destDir%\maps\OshimaCore.dll" /Y >nul
copy "%sourceDir%\OshimaMaps.dll"  "%destDir%\maps\OshimaMaps.dll"  /Y >nul
if not exist "%destDir%\modules" mkdir "%destDir%\modules"
copy "%sourceDir%\OshimaCore.dll"    "%destDir%\modules\OshimaCore.dll"    /Y >nul
copy "%sourceDir%\OshimaMaps.dll"    "%destDir%\modules\OshimaMaps.dll"    /Y >nul
copy "%sourceDir%\OshimaModules.dll" "%destDir%\modules\OshimaModules.dll" /Y >nul
copy "%sourceDir%\OshimaServers.dll" "%destDir%\modules\OshimaServers.dll" /Y >nul
if not exist "%destDir2%" mkdir "%destDir2%"
copy "%sourceDir%\OshimaCore.dll"    "%destDir2%\OshimaCore.dll"    /Y >nul
copy "%sourceDir%\OshimaMaps.dll"    "%destDir2%\OshimaMaps.dll"    /Y >nul
copy "%sourceDir%\OshimaModules.dll" "%destDir2%\OshimaModules.dll" /Y >nul
copy "%sourceDir%\OshimaServers.dll" "%destDir2%\OshimaServers.dll" /Y >nul
if not exist "%destDir3%" mkdir "%destDir3%"
copy "%sourceDir%\OshimaCore.dll"    "%destDir3%\OshimaCore.dll"    /Y >nul
copy "%sourceDir%\OshimaMaps.dll"    "%destDir3%\OshimaMaps.dll"    /Y >nul
copy "%sourceDir%\OshimaModules.dll" "%destDir3%\OshimaModules.dll" /Y >nul
copy "%sourceDir%\OshimaServers.dll" "%destDir3%\OshimaServers.dll" /Y >nul
copy "%sourceDir%\OshimaWebAPI.dll"  "%destDir3%\OshimaWebAPI.dll"  /Y >nul
if not exist "%destDir%\configs" mkdir "%destDir%\configs"
xcopy "%sourceDir%\configs\" "%destDir%\configs\" /e /i /y /q >nul

cd FunGame.Server\bin\Debug\net10.0
start FunGameWebAPI.exe