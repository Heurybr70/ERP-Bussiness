# Consulte https://aka.ms/customizecontainer para aprender a personalizar su contenedor de depuración y cómo Visual Studio usa este Dockerfile para compilar sus imágenes para una depuración más rápida.

# Esta fase se usa cuando se ejecuta desde VS en modo rápido (valor predeterminado para la configuración de depuración)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# Esta fase se usa para compilar el proyecto de servicio
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Platform.Api/Platform.Api.csproj", "Platform.Api/"]
COPY ["Modules.Billing/Modules.Billing.csproj", "Modules.Billing/"]
COPY ["Platform.Core/Platform.Core.csproj", "Platform.Core/"]
COPY ["Modules.EInvoicing/Modules.EInvoicing.csproj", "Modules.EInvoicing/"]
COPY ["Modules.Inventory/Modules.Inventory.csproj", "Modules.Inventory/"]
COPY ["Modules.Metrics/Modules.Metrics.csproj", "Modules.Metrics/"]
COPY ["Modules.Quotes/Modules.Quotes.csproj", "Modules.Quotes/"]
COPY ["Modules.Reporting/Modules.Reporting.csproj", "Modules.Reporting/"]
RUN dotnet restore "./Platform.Api/Platform.Api.csproj"
COPY . .
WORKDIR "/src/Platform.Api"
RUN dotnet build "./Platform.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Esta fase se usa para publicar el proyecto de servicio que se copiará en la fase final.
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Platform.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Esta fase se usa en producción o cuando se ejecuta desde VS en modo normal (valor predeterminado cuando no se usa la configuración de depuración)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Platform.Api.dll"]