@Echo Off

CALL Config.bat

"%vs_basedir%devenv" /build release ../proj/SecurityServer/proj/SecurityServer.sln

IF %ERRORLEVEL% NEQ 0 (
  echo. && echo. && echo Build failed.
  exit /B 1
)

%nuget_basedir%nuget.exe pack Dragon.SecurityServer.AccountSTS.Client.nuspec
%nuget_basedir%nuget.exe pack Dragon.SecurityServer.ProfileSTS.Client.nuspec
REM %nuget_basedir%nuget.exe pack Dragon.SecurityServer.PermissionSTS.Client.nuspec

echo.
echo.
echo To publish, run:
echo %nuget_basedir%nuget push Dragon.SecurityServer.AccountSTS.Client*.nupkg %nuget_args%
echo %nuget_basedir%nuget push Dragon.SecurityServer.ProfileSTS.Client*.nupkg %nuget_args%
REM %nuget_basedir%nuget push Dragon.SecurityServer.PermissionSTS.Client*.nupkg
