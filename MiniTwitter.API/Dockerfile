# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app

EXPOSE 8080



# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

# Disable fallback packages to avoid Windows path issues in Linux container
ENV NUGET_FALLBACK_PACKAGES=""

# Add EF CLI tool to the SDK image
ENV PATH="$PATH:/root/.dotnet/tools"
RUN dotnet tool install --global dotnet-ef


ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["MiniTwitter.API/MiniTwitter.API.csproj", "MiniTwitter.API/"]
COPY ["MiniTwitter.Data/MiniTwitter.Data.csproj", "MiniTwitter.Data/"]
COPY ["MiniTwitter.Core/MiniTwitter.Core.csproj", "MiniTwitter.Core/"]
COPY ["MiniTwitter.Infrastructure/MiniTwitter.Infrastructure.csproj", "MiniTwitter.Infrastructure/"]
COPY ["MiniTwitter.Service/MiniTwitter.Service.csproj", "MiniTwitter.Service/"]
RUN dotnet restore "./MiniTwitter.API/MiniTwitter.API.csproj"
COPY . .
WORKDIR "/src/MiniTwitter.API"
RUN dotnet build "./MiniTwitter.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MiniTwitter.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY MiniTwitter.API/appsettings.json ./appsettings.json
ENTRYPOINT ["dotnet", "MiniTwitter.API.dll"]

# --- New migration stage for running EF migrations ---
FROM build AS migrate-env
WORKDIR /src/MiniTwitter.API