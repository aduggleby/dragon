@Echo Off

CALL Config.bat

"%vs_basedir%devenv" /build release ../proj/Files/Dragon.Files.sln
IF %ERRORLEVEL% NEQ 0 (
  echo. && echo. && echo Build failed.
  exit /B 1
)

REM "%ProgramFiles(x86)%/Microsoft Visual Studio 12.0/Common7/IDE/mstest" /testcontainer:"../proj/Files/test/bin/Release/Dragon.Files.Test.dll"
REM TODO: does not find app.config

%nuget_basedir%nuget.exe pack Dragon.Files.nuspec
%nuget_basedir%nuget.exe pack Dragon.Files.MVC.nuspec
%nuget_basedir%nuget.exe pack Dragon.Files.S3.nuspec
%nuget_basedir%nuget.exe pack Dragon.Files.AzureBlobStorage.nuspec

echo.
echo.
echo To publish, run:
echo %nuget_basedir%nuget push Dragon.Files.AzureBlobStorage*.nupkg %nuget_args%
echo %nuget_basedir%nuget push Dragon.Files.S3*.nupkg %nuget_args%
echo %nuget_basedir%nuget push Dragon.Files.MVC*.nupkg %nuget_args%
echo %nuget_basedir%nuget push Dragon.Files*.nupkg %nuget_args%
