FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["RoomService.API/RoomService.API.csproj", "RoomService.API/"]
COPY ["RoomService.Infrastructure/RoomService.Infrastructure.csproj", "RoomService.Infrastructure/"]
COPY ["RoomService.Application/RoomService.Application.csproj", "RoomService.Application/"]
COPY ["RoomService.Domain/RoomService.Domain.csproj", "RoomService.Domain/"]
RUN dotnet restore "RoomService.API/RoomService.API.csproj"
COPY . .
WORKDIR "/src/RoomService.API"
RUN dotnet build "./RoomService.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./RoomService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

USER root
RUN mkdir -p /app/Logs && chown -R app:app /app/Logs
USER app

ENTRYPOINT ["dotnet", "RoomService.API.dll"]
