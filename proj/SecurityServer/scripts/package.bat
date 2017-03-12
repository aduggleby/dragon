@Echo Off

set projects=(AccountSTS PermissionSTS ProfileSTS)
set export_dir=..\..\package

pushd proj

echo building solution...

"%ProgramFiles(x86)%/Microsoft Visual Studio 12.0/Common7/IDE/devenv" SecurityServer.sln /Rebuild Release
IF %ERRORLEVEL% NEQ 0 (
    echo. && echo. && echo Build failed.
    exit /B 1
)

for %%i in %projects% do (
    echo exporting %%i...
    pushd %%i
    if not exist %export_dir% mkdir %export_dir%
    git archive master -o %export_dir%/%%i.zip
    popd
)

popd

echo done.