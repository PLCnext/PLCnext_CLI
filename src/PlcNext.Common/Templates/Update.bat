@echo off

setlocal EnableDelayedExpansion
for %%a in (*.xsd) do (
	set file=%%~a
	set subNamespace=%%~na
	echo ------------------------------------------------------------
	echo Update !file!
	echo ------------------------------------------------------------
	set subNamespace="!subNamespace:Schema=!"
	if not !subNamespace!=="Templates" (
	set subNamespace="!subNamespace:Template=!"
	)
    "C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.7.2 Tools\xsd.exe" /c /n:PlcNext.Common.Templates.!subNamespace! !file!
	echo.
)

pause