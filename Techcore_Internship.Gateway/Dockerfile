# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 5000


FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Techcore_Internship.Gateway/Techcore_Internship.Gateway.csproj", "Techcore_Internship.Gateway/"]
RUN dotnet restore "./Techcore_Internship.Gateway/Techcore_Internship.Gateway.csproj"
COPY . .
WORKDIR "/src/Techcore_Internship.Gateway"
RUN dotnet build "./Techcore_Internship.Gateway.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Techcore_Internship.Gateway.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY ["Techcore_Internship.Gateway/ocelot.local.json", "./"]
COPY ["Techcore_Internship.Gateway/ocelot.docker.json", "./"]
COPY ["Techcore_Internship.Gateway/appsettings.json", "./"]
COPY ["Techcore_Internship.Gateway/appsettings.Development.json", "./"]
ENTRYPOINT ["dotnet", "Techcore_Internship.Gateway.dll"]