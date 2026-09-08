FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Packages.props", "./"]
COPY ["src/Services.API/Services.API.API/InnoClinic.Services.API.csproj", "src/Services.API/Services.API.API/"]
COPY ["src/Services.API/Services.API.Application/InnoClinic.Services.API.Application.csproj", "src/Services.API/Services.API.Application/"]
COPY ["src/Services.API/Services.API.Domain/InnoClinic.Services.API.Domain.csproj", "src/Services.API/Services.API.Domain/"]
COPY ["src/Services.API/Services.API.Infrastructure/InnoClinic.Services.API.Infrastructure.csproj", "src/Services.API/Services.API.Infrastructure/"]
COPY ["src/Shared/InnoClinic.Shared.csproj", "src/Shared/"]
RUN dotnet restore "./src/Services.API/Services.API.API/InnoClinic.Services.API.csproj"
COPY . .
WORKDIR "/src/src/Services.API/Services.API.API"
RUN dotnet build "./InnoClinic.Services.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./InnoClinic.Services.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "InnoClinic.Services.API.dll"]