FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["MusicService.API/MusicService.API.csproj", "MusicService.API/"]
COPY ["MusicService.Infrastructure/MusicService.Infrastructure.csproj", "MusicService.Infrastructure/"]
COPY ["MusicService.Application/MusicService.Application.csproj", "MusicService.Application/"]
COPY ["MusicService.Domain/MusicService.Domain.csproj", "MusicService.Domain/"]
RUN dotnet restore "MusicService.API/MusicService.API.csproj"
COPY . .
WORKDIR "/src/MusicService.API"
RUN dotnet build "./MusicService.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MusicService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

USER root
RUN mkdir -p /app/Musics && chown -R app:app /app/Musics
RUN mkdir -p /app/Logs && chown -R app:app /app/Logs
USER app

ENTRYPOINT ["dotnet", "MusicService.API.dll"]
