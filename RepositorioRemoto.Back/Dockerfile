FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["RepositorioRemoto/RepositorioRemoto.csproj", "RepositorioRemoto/"]
RUN dotnet restore "RepositorioRemoto/RepositorioRemoto.csproj"
COPY . .
WORKDIR "/src/RepositorioRemoto"
RUN dotnet build "./RepositorioRemoto.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./RepositorioRemoto.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RepositorioRemoto.dll"]
