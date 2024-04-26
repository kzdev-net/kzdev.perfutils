@ECHO OFF
SETLOCAL
SET SOLUTIONPATH=%~dp0..\..

PUSHD %SOLUTIONPATH%

dotnet clean -c Release
dotnet restore

POPD
 
dotnet msbuild -t:Pack -p:IsPacking=true -p:Configuration=Release -p:ContinuousIntegrationBuild=true

:EOF
ENDLOCAL
