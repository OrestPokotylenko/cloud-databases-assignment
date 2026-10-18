FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

ARG BUILD_CONFIGURATION=Release

WORKDIR /source

COPY global.json Directory.Packages.props ./
COPY src/OnlineStore.API/OnlineStore.API.csproj src/OnlineStore.API/
COPY src/OnlineStore.Application/OnlineStore.Application.csproj src/OnlineStore.Application/
COPY src/OnlineStore.Domain/OnlineStore.Domain.csproj src/OnlineStore.Domain/
COPY src/OnlineStore.ExternalServices/OnlineStore.ExternalServices.csproj src/OnlineStore.ExternalServices/
COPY src/OnlineStore.Persistence/OnlineStore.Persistence.csproj src/OnlineStore.Persistence/

RUN dotnet restore src/OnlineStore.API/OnlineStore.API.csproj

COPY src/ src/

RUN dotnet publish src/OnlineStore.API/OnlineStore.API.csproj \
    --configuration $BUILD_CONFIGURATION \
    --no-restore \
    --output /app/publish \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

ENV ASPNETCORE_URLS=http://+:8080 \
    DOTNET_EnableDiagnostics=0

WORKDIR /app
COPY --from=build /app/publish .

USER $APP_UID
EXPOSE 8080

ENTRYPOINT ["dotnet", "OnlineStore.API.dll"]
