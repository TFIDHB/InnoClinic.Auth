# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Packages.props", "./"]
COPY ["src/Profiles.API/InnoClinic.Profiles.API/InnoClinic.Profiles.API.csproj", "src/Profiles.API/InnoClinic.Profiles.API/"]
COPY ["src/Shared/InnoClinic.Shared.csproj", "src/Shared/"]
COPY ["src/Profiles.API/Profiles.API.Infrastructure/InnoClinic.Profiles.API.Infrastructure.csproj", "src/Profiles.API/Profiles.API.Infrastructure/"]
COPY ["src/Profiles.API/Profiles.API.Application/InnoClinic.Profiles.API.Application.csproj", "src/Profiles.API/Profiles.API.Application/"]
COPY ["src/Profiles.API/Profiles.API.Domain/InnoClinic.Profiles.API.Domain.csproj", "src/Profiles.API/Profiles.API.Domain/"]
RUN dotnet restore "./src/Profiles.API/InnoClinic.Profiles.API/InnoClinic.Profiles.API.csproj"
COPY . .
WORKDIR "/src/src/Profiles.API/InnoClinic.Profiles.API"
RUN dotnet build "./InnoClinic.Profiles.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./InnoClinic.Profiles.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "InnoClinic.Profiles.API.dll"]