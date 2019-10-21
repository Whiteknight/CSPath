dotnet build CSPath\CSPath.csproj --configuration Release
if ERRORLEVEL 1 GOTO :error

dotnet pack CSPath\CSPath.csproj --configuration Release --no-build --no-restore
if ERRORLEVEL 1 GOTO :error

goto :done

:error
echo Build FAILED

:done