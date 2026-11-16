FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Inventory.slnx ./
COPY src/Inventory.Core/Inventory.Core.csproj src/Inventory.Core/
COPY src/Inventory.Infrastructure/Inventory.Infrastructure.csproj src/Inventory.Infrastructure/
COPY src/Inventory.Api/Inventory.Api.csproj src/Inventory.Api/
RUN dotnet restore src/Inventory.Api/Inventory.Api.csproj

COPY src/ src/
RUN dotnet publish src/Inventory.Api/Inventory.Api.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app .
EXPOSE 8080
USER $APP_UID
ENTRYPOINT ["dotnet", "Inventory.Api.dll"]
