@Echo Off

CALL Config.bat

"%vs_basedir%devenv" /build release ../proj/Security.Hmac/Dragon.Security.Hmac.sln
IF %ERRORLEVEL% NEQ 0 (
  echo. && echo. && echo Build failed.
  exit /B 1
)

%nuget_basedir%nuget.exe pack Dragon.Security.Hmac.Core.nuspec
%nuget_basedir%nuget.exe pack Dragon.Security.Hmac.Module.nuspec

echo.
echo.
echo To publish, run:
echo %nuget_basedir%nuget push Dragon.Security.Hmac.Core*.nupkg %nuget_args%
echo %nuget_basedir%nuget push Dragon.Security.Hmac.Module*.nupkg %nuget_args%
