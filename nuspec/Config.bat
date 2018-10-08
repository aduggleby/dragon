@Echo Off

REM set your api key once: nuget setApiKey Your-API-Key

SET "nuget_basedir="
SET "nuget_args=-Source https://api.nuget.org/v3/index.json"

SET "vs_basedir=%ProgramFiles(x86)%/Microsoft Visual Studio 14.0/Common7/IDE/"
IF NOT EXIST "%vs_basedir%devenv" SET "vs_basedir=%ProgramFiles(x86)%/Microsoft Visual Studio/2017/Community/Common7/IDE/"
