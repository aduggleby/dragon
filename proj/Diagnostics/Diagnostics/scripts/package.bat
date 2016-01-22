pushd ..\bin\
rm Dragon.Diagnostics.*
"%programfiles(x86)%\Microsoft\ILMerge\IlMerge" /target:exe /targetplatform:"v4,C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0" /out:Dragon.Diagnostics.exe Release\Dragon.Diagnostics.exe Release\CommandLine.dll Release\Division42.NetworkTools.dll Release\websocket-sharp.dll
"%programfiles%/7-Zip/7z.exe" a -tzip Dragon.Diagnostics.zip Dragon.Diagnostics.exe
popd
mv ..\bin\Dragon.Diagnostics.zip .

set /p DUMMY=Hit ENTER to continue...
