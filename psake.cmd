@echo off

if '%1'=='/?' goto help
if '%1'=='-help' goto help
if '%1'=='-h' goto help

powershell -NoProfile -ExecutionPolicy Bypass -Command "& '%~dp0\tools\psake.4.2.0.1\psake.ps1' %*; if ($psake.build_success -eq $false) { exit 1 } else { exit 0 }"
goto :eof

:help
powershell -NoProfile -ExecutionPolicy Bypass -Command "& '%~dp\tools\psake.4.2.0.1\psake.ps1' -help"
