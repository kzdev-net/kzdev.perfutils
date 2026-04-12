# Linux test image: .NET SDK 10 matches Source/global.json and CI (Build / Test workflow).
# The SDK restores, builds, and tests net8.0, net9.0, and net10.0 — the package target frameworks
# defined under Source/Src and Source/Tst Directory.Build.props. `dotnet test` on the solution
# runs the standard test suite once per target framework for each multi-targeted test project.
FROM mcr.microsoft.com/dotnet/sdk:10.0

ENV DOTNET_CLI_TELEMETRY_OPTOUT=1 \
    DOTNET_NOLOGO=1

# Set the working directory inside the container
WORKDIR /app

# Build context is the Source directory (see Tst/README.md).
COPY . .

# Restore the project dependencies
RUN dotnet restore KZDev.PerfUtils.slnx

# Build the project
RUN dotnet build KZDev.PerfUtils.slnx -c Release --no-restore

# Standard tests only (same as CI job test-standard); all TFMs; Release must match build above.
CMD ["dotnet", "test", "KZDev.PerfUtils.slnx", "-c", "Release", "--no-build", "--logger", "xunit;LogFileName={assembly}.{framework}.results.xml", "--results-directory", "/app/Tst/linux-tests-results"]
