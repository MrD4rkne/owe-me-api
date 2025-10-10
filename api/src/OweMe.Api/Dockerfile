FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release

COPY ./api/ /api
COPY ./client/ /client
COPY ./Directory.Build.props /
COPY ./Directory.Packages.props /
COPY OweMe.Api.sln /
COPY version.json /

WORKDIR /
RUN dotnet restore OweMe.Api.sln
WORKDIR /api/src/OweMe.Api
RUN dotnet build -c "$BUILD_CONFIGURATION" --no-restore -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "OweMe.Api.csproj" -c "$BUILD_CONFIGURATION" -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
USER $APP_UID
ENTRYPOINT ["dotnet", "OweMe.Api.dll"]
