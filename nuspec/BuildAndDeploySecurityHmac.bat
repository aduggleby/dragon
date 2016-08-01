@Echo Off

REM set your api key once: nuget setApiKey Your-API-Key

SET "nuget_basedir="

"%ProgramFiles(x86)%/Microsoft Visual Studio 14.0/Common7/IDE/devenv" /build release ../proj/Security.Hmac/Dragon.Security.Hmac.sln
IF %ERRORLEVEL% NEQ 0 (
  echo. && echo. && echo Build failed.
  exit /B 1
)

%nuget_basedir%nuget.exe pack Dragon.Security.Hmac.Core.nuspec
%nuget_basedir%nuget.exe pack Dragon.Security.Hmac.Module.nuspec

%nuget_basedir%nuget push Dragon.Security.Hmac.Core*.nupkg
%nuget_basedir%nuget push Dragon.Security.Hmac.Module*.nupkg

