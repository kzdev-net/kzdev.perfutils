@ECHO OFF
SETLOCAL
SET "PROJECTPATH=%~dp0KZDev.PerfUtils.csproj"
SET SOLUTIONPATH=%~dp0..\..

PUSHD %SOLUTIONPATH%

dotnet clean "%PROJECTPATH%" -c Release
dotnet restore "%PROJECTPATH%"
dotnet pack "%PROJECTPATH%" -c Release --no-restore -p:IsPacking=true -p:ContinuousIntegrationBuild=true

POPD

:EOF
ENDLOCAL
