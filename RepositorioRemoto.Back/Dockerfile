FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["RepositorioRemoto.Back/RepositorioRemoto.Back.csproj", "RepositorioRemoto.Back/"]

RUN dotnet restore "RepositorioRemoto.Back/RepositorioRemoto.Back.csproj"

COPY . .

WORKDIR "/src/RepositorioRemoto.Back"

RUN dotnet build "RepositorioRemoto.Back.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release

RUN dotnet publish "RepositorioRemoto.Back.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    /p:UseAppHost=false

FROM base AS final
WORKDIR /app

COPY --from=publish /app/publish .

RUN mkdir -p /app/data && chmod -R 777 /app/data

ENTRYPOINT ["dotnet", "RepositorioRemoto.Back.dll"]