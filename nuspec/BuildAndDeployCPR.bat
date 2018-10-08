@Echo Off

CALL Config.bat

"%vs_basedir%devenv" /build release ../proj/CPR/Dragon.CPR.sln
IF %ERRORLEVEL% NEQ 0 (
  echo. && echo. && echo Build failed.
  exit /B 1
)

%nuget_basedir%nuget.exe pack Dragon.CPR.nuspec

echo.
echo.
echo To publish, run:
echo %nuget_basedir%nuget push Dragon.CPR.*.nupkg %nuget_args%
