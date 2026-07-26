FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081
# Katalog na key ring Data Protection. Tworzony jako root i oddany użytkownikowi
# aplikacji, bo wolumen podpięty w compose.yaml dziedziczy właściciela z obrazu
# przy pierwszej inicjalizacji — inaczej kontener (non-root) nie mógłby tu pisać.
RUN mkdir -p /keys && chown $APP_UID /keys
USER $APP_UID

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["dockerbiblioteka/dockerbiblioteka.csproj", "dockerbiblioteka/"]
RUN dotnet restore "dockerbiblioteka/dockerbiblioteka.csproj"
COPY . .
WORKDIR "/src/dockerbiblioteka"
RUN dotnet build "./dockerbiblioteka.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./dockerbiblioteka.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "dockerbiblioteka.dll"]
