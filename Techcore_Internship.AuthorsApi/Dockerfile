# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 5002


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Techcore_Internship.AuthorsApi/Techcore_Internship.AuthorsApi.csproj", "Techcore_Internship.AuthorsApi/"]
COPY ["Techcore_Internship.Data/Techcore_Internship.Data.csproj", "Techcore_Internship.Data/"]
COPY ["Techcore_Internship.Contracts/Techcore_Internship.Contracts.csproj", "Techcore_Internship.Contracts/"]
COPY ["Techcore_Internship.Domain/Techcore_Internship.Domain.csproj", "Techcore_Internship.Domain/"]
RUN dotnet restore "./Techcore_Internship.AuthorsApi/Techcore_Internship.AuthorsApi.csproj"
COPY . .
WORKDIR "/src/Techcore_Internship.AuthorsApi"
RUN dotnet build "./Techcore_Internship.AuthorsApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Techcore_Internship.AuthorsApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Techcore_Internship.AuthorsApi.dll"]