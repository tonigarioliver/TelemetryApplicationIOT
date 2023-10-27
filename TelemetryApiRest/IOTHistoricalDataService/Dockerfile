# Use the base image for building
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

# Copy the csproj and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy the application files
COPY . ./
RUN dotnet publish -c Release -o out

# Build the final runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final-env
WORKDIR /app

# Copy appsettings.json to the image
COPY appsettings.json /app/appsettings.json
COPY appsettings.Docker.json /app/appsettings.Docker.json

ENV ASPNETCORE_ENVIRONMENT=Docker

# Copy application files from the build image
COPY --from=build-env /app/out .

# Set the entry point for your application
ENTRYPOINT ["dotnet", "IOTHistoricalDataService.dll"]
