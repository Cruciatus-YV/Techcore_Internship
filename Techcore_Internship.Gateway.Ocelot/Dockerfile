FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 5005
ENV ASPNETCORE_URLS=http://+:5005

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Techcore_Internship.Gateway.Ocelot/Techcore_Internship.Gateway.Ocelot.csproj", "Techcore_Internship.Gateway.Ocelot/"]
RUN dotnet restore "./Techcore_Internship.Gateway.Ocelot/Techcore_Internship.Gateway.Ocelot.csproj"
COPY . .
WORKDIR "/src/Techcore_Internship.Gateway.Ocelot"
RUN dotnet build "./Techcore_Internship.Gateway.Ocelot.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Techcore_Internship.Gateway.Ocelot.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Techcore_Internship.Gateway.Ocelot.dll"]