@Echo Off

REM set your api key once: nuget setApiKey Your-API-Key

SET "nuget_basedir="

"%ProgramFiles(x86)%/Microsoft Visual Studio 14.0/Common7/IDE/devenv" /build release ../proj/SecurityServer/proj/SecurityServer.sln
IF %ERRORLEVEL% NEQ 0 (
  echo. && echo. && echo Build failed.
  exit /B 1
)

%nuget_basedir%nuget.exe pack Dragon.SecurityServer.AccountSTS.Client.nuspec
%nuget_basedir%nuget.exe pack Dragon.SecurityServer.ProfileSTS.Client.nuspec

%nuget_basedir%nuget push Dragon.SecurityServer.AccountSTS.Client*.nupkg
REM %nuget_basedir%nuget push Dragon.SecurityServer.ProfileSTS.Client*.nupkg