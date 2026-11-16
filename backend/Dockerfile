FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Inventory.slnx ./
COPY src/Inventory.Core/Inventory.Core.csproj src/Inventory.Core/
COPY src/Inventory.Infrastructure/Inventory.Infrastructure.csproj src/Inventory.Infrastructure/
COPY src/Inventory.Api/Inventory.Api.csproj src/Inventory.Api/
RUN dotnet restore src/Inventory.Api/Inventory.Api.csproj

COPY src/ src/
RUN dotnet publish src/Inventory.Api/Inventory.Api.csproj -c Release -o /app --no-restore

FROM build AS bundle
COPY dotnet-tools.json ./
RUN dotnet tool restore
# EF tooling executes Program.cs, which validates these settings; the values are build-time placeholders only
RUN ConnectionStrings__Default="Server=placeholder" \
    Jwt__Key="build-time-placeholder-key-not-used-at-runtime" \
    Jwt__Issuer="placeholder" \
    Jwt__Audience="placeholder" \
    Cors__AllowedOrigins__0="http://localhost" \
    dotnet ef migrations bundle \
    --project src/Inventory.Infrastructure \
    --startup-project src/Inventory.Api \
    --self-contained -r linux-x64 \
    -o /bundle/efbundle

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS migrate
WORKDIR /app
COPY --from=bundle /bundle/efbundle ./
COPY src/Inventory.Api/appsettings.json ./
USER $APP_UID
ENTRYPOINT ["./efbundle"]

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app .
EXPOSE 8080
USER $APP_UID
ENTRYPOINT ["dotnet", "Inventory.Api.dll"]
