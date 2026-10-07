FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# 1. Copiar el csproj directamente
COPY ["RepositorioRemoto.Back.csproj", "./"]

# 2. Restaurar dependencias
RUN dotnet restore "RepositorioRemoto.Back.csproj"

# 3. Copiar todo el contenido del proyecto
COPY . .

# 4. Compilar el proyecto
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

# Copiar archivos publicados
COPY --from=publish /app/publish .

# Crear el directorio /app/data para SQLite y asignar permisos
RUN mkdir -p /app/data && chown -R app:app /app && chmod -R 777 /app/data

USER app

# Para Development
ENTRYPOINT ["dotnet", "RepositorioRemoto.Back.dll", "Production"]