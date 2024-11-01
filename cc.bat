@echo off
setlocal

set EXE=CookieCode.DotNetTools/bin/Debug/net8.0/dnt.exe
set SHOULD_WAIT=1

if %SHOULD_WAIT%==1 (
	"%EXE%" %*
) else (
	start "" "%EXE%" %*
)

endlocal
