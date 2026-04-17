FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["CommentService.API/CommentService.API.csproj", "CommentService.API/"]
COPY ["CommentService.Infrastructure/CommentService.Infrastructure.csproj", "CommentService.Infrastructure/"]
COPY ["CommentService.Application/CommentService.Application.csproj", "CommentService.Application/"]
COPY ["CommentService.Domain/CommentService.Domain.csproj", "CommentService.Domain/"]
RUN dotnet restore "CommentService.API/CommentService.API.csproj"
COPY . .
WORKDIR "/src/CommentService.API"
RUN dotnet build "./CommentService.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CommentService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

USER root
RUN apt-get update && apt-get install -y ffmpeg && rm -rf /var/lib/apt/lists/*
RUN mkdir -p /app/Videos && chown -R app:app /app/Videos
RUN mkdir -p /app/Pictures && chown -R app:app /app/Pictures
USER app

ENTRYPOINT ["dotnet", "CommentService.API.dll"]
