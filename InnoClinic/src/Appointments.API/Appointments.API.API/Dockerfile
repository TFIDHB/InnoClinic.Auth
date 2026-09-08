FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Packages.props", "./"]
COPY ["src/Appointments.API/Appointments.API.API/InnoClinic.Appointments.API.csproj", "src/Appointments.API/Appointments.API.API/"]
COPY ["src/Appointments.API/Appointments.API.Application/InnoClinic.Appointments.API.Application.csproj", "src/Appointments.API/Appointments.API.Application/"]
COPY ["src/Appointments.API/Appointments.API.Domain/InnoClinic.Appointments.API.Domain.csproj", "src/Appointments.API/Appointments.API.Domain/"]
COPY ["src/Appointments.API/Appointments.API.Infrastructure/InnoClinic.Appointments.API.Infrastructure.csproj", "src/Appointments.API/Appointments.API.Infrastructure/"]
COPY ["src/Shared/InnoClinic.Shared.csproj", "src/Shared/"]
RUN dotnet restore "./src/Appointments.API/Appointments.API.API/InnoClinic.Appointments.API.csproj"
COPY . .
WORKDIR "/src/src/Appointments.API/Appointments.API.API"
RUN dotnet build "./InnoClinic.Appointments.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./InnoClinic.Appointments.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "InnoClinic.Appointments.API.dll"]