@ECHO OFF
set SOURCE_FOLDER=%1
set SOURCE_FOLDER=%SOURCE_FOLDER:"=%
set DESTINATION_FOLDER=%2
set DESTINATION_FOLDER=%DESTINATION_FOLDER:"=%
set TEMP_FOLDER=%3
set TEMP_FOLDER=%TEMP_FOLDER:"=%
set TASK_NAME=%4
set TASK_NAME=%TASK_NAME:"=%
TIMEOUT /t 1 /nobreak > NUL
TASKKILL /IM %TASK_NAME% > NUL
XCOPY "%SOURCE_FOLDER%\*" "%DESTINATION_FOLDER%" /S /I /Y /R > NUL
Pushd "%TEMP_FOLDER%"
Del /f /q /s *.* >NUL
CD \
RD /s /q "%TEMP_FOLDER%"
:: repeat because RD is sometimes buggy 
if exist "%TEMP_FOLDER%" RD /s /q "%TEMP_FOLDER%"
Popd