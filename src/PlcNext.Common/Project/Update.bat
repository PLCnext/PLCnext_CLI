@echo off

setlocal EnableDelayedExpansion
for %%a in (*.xsd) do (
	set file=%%~a
	echo ------------------------------------------------------------
	echo Update !file!
	echo ------------------------------------------------------------
    "C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.7.1 Tools\xsd.exe" /c /n:PlcNext.Common.Project !file!
	echo.
)

pause