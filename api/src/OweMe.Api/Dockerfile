FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

EXPOSE 8080
EXPOSE 8081
USER $APP_UID

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy only project files first for better layer caching
COPY version.json ./
COPY OweMe.Api.sln ./
COPY Directory.Build.props ./
COPY Directory.Packages.props ./

# Copy all .csproj files to restore dependencies
COPY api/src/OweMe.Api/*.csproj ./api/src/OweMe.Api/
COPY api/src/OweMe.Application/*.csproj ./api/src/OweMe.Application/
COPY api/src/OweMe.Domain/*.csproj ./api/src/OweMe.Domain/
COPY api/src/OweMe.Infrastructure/*.csproj ./api/src/OweMe.Infrastructure/
COPY api/src/OweMe.Persistence/*.csproj ./api/src/OweMe.Persistence/
COPY client/OweMe.Api.Client/*.csproj ./client/OweMe.Api.Client/

# Restore only the main project dependencies (excluding test projects)
RUN dotnet restore ./api/src/OweMe.Api/OweMe.Api.csproj
RUN dotnet restore ./client/OweMe.Api.Client/OweMe.Api.Client.csproj

# Copy remaining source files
COPY api/ ./api/
COPY client/ ./client/

# Build the project
WORKDIR /src/api/src/OweMe.Api
RUN dotnet build "OweMe.Api.csproj" -c "$BUILD_CONFIGURATION" --no-restore -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "OweMe.Api.csproj" -c "$BUILD_CONFIGURATION" --no-restore -o /app/publish /p:UseAppHost=false

FROM base AS final
USER $APP_UID
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OweMe.Api.dll"]
