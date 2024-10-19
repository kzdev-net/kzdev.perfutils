# Use the official .NET 6 SDK image as the base image for .NET 6
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS dotnet6

# Use the official .NET 8 SDK image as the base image for .NET 8
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS final

# Install .NET 6 SDK
COPY --from=dotnet6 /usr/share/dotnet /usr/share/dotnet
COPY --from=dotnet6 /etc/ssl/certs/ca-certificates.crt /etc/ssl/certs/

# Set the working directory inside the container
WORKDIR /app

# Copy the project files to the container
COPY . .

# Restore the project dependencies
RUN dotnet restore

# Build the project
RUN dotnet build --no-restore

# Run the tests
CMD ["dotnet", "test", "--no-build", "--logger", "xunit;LogFileName={assembly}.{framework}.results.xml", "--results-directory", "/app/Tst/linux-tests-results"]
